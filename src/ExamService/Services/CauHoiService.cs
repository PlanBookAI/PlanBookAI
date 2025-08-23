using AutoMapper;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OfficeOpenXml; // Thêm using cho EPPlus
using OfficeOpenXml.Style; // Thêm using cho style
using System.Drawing; // Thêm using cho Color

namespace ExamService.Services
{
    public class CauHoiService : ICauHoiService
    {
        private readonly ICauHoiRepository _repo;
        private readonly IMapper _mapper;

        public CauHoiService(ICauHoiRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiPhanHoi<PagedResult<CauHoiResponseDTO>>> GetAllAsync(Guid teacherId, PagingDTO pagingParams)
        {
            var query = _repo.GetQueryable().Where(c => c.NguoiTaoId == teacherId);

            // TODO: Implement Filtering, Sorting, Searching here

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                                   .Take(pagingParams.PageSize)
                                   .Include(c => c.LuaChons)
                                   .ToListAsync();

            var dtos = _mapper.Map<List<CauHoiResponseDTO>>(items);
            var pagedResult = new PagedResult<CauHoiResponseDTO>(dtos, totalItems, pagingParams.PageNumber, pagingParams.PageSize);

            return ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThanhCongVoiDuLieu(pagedResult);
        }

        public async Task<ApiPhanHoi<CauHoiResponseDTO>> GetByIdAsync(Guid id, Guid teacherId)
        {
            var cauHoi = await _repo.GetByIdAsync(id);
            if (cauHoi == null || cauHoi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<CauHoiResponseDTO>.ThatBai("Không tìm thấy câu hỏi hoặc không có quyền truy cập.");
            }
            var responseDto = _mapper.Map<CauHoiResponseDTO>(cauHoi);
            return ApiPhanHoi<CauHoiResponseDTO>.ThanhCongVoiDuLieu(responseDto);
        }

        public async Task<ApiPhanHoi<CauHoiResponseDTO>> CreateAsync(CauHoiRequestDTO dto, Guid teacherId)
        {
            var cauHoi = _mapper.Map<CauHoi>(dto);
            cauHoi.NguoiTaoId = teacherId;
            cauHoi.Id = Guid.NewGuid();

            var newCauHoi = await _repo.CreateAsync(cauHoi);
            var responseDto = _mapper.Map<CauHoiResponseDTO>(newCauHoi);

            return ApiPhanHoi<CauHoiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Tạo câu hỏi thành công.");
        }

        public async Task<ApiPhanHoi<CauHoiResponseDTO>> UpdateAsync(Guid id, CauHoiRequestDTO dto, Guid teacherId)
        {
            var existingCauHoi = await _repo.GetByIdAsync(id);
            if (existingCauHoi == null || existingCauHoi.NguoiTaoId != teacherId)
            {
                return ApiPhanHoi<CauHoiResponseDTO>.ThatBai("Không tìm thấy câu hỏi hoặc không có quyền truy cập.");
            }

            // Update properties
            _mapper.Map(dto, existingCauHoi);

            // Handle choices update (remove old, add new)
            existingCauHoi.LuaChons.Clear();
            foreach (var luaChonDto in dto.LuaChons)
            {
                existingCauHoi.LuaChons.Add(_mapper.Map<LuaChon>(luaChonDto));
            }

            existingCauHoi.CapNhatLuc = DateTime.UtcNow;

            var updatedCauHoi = await _repo.UpdateAsync(existingCauHoi);
            var responseDto = _mapper.Map<CauHoiResponseDTO>(updatedCauHoi);
            return ApiPhanHoi<CauHoiResponseDTO>.ThanhCongVoiDuLieu(responseDto, "Cập nhật câu hỏi thành công.");
        }

        public async Task<ApiPhanHoi<bool>> DeleteAsync(Guid id, Guid teacherId)
        {
            if (!await _repo.IsOwnerAsync(id, teacherId))
            {
                return ApiPhanHoi<bool>.ThatBai("Không tìm thấy câu hỏi hoặc không có quyền truy cập.");
            }

            var result = await _repo.DeleteAsync(id);
            if (!result)
            {
                return ApiPhanHoi<bool>.ThatBai("Xóa câu hỏi thất bại.");
            }
            return ApiPhanHoi<bool>.ThanhCongVoiDuLieu(true, "Xóa câu hỏi thành công.");
        }

        public async Task<ApiPhanHoi<PagedResult<CauHoiResponseDTO>>> SearchAsync(Guid teacherId, CauHoiSearchParametersDTO searchParams)
        {
            // Bắt đầu với một IQueryable cơ bản, đã lọc theo giáo viên
            var query = _repo.GetQueryable().Where(c => c.NguoiTaoId == teacherId);

            // 1. Lọc (Filtering)
            if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
            {
                query = query.Where(c => c.NoiDung.ToLower().Contains(searchParams.Keyword.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchParams.MonHoc))
            {
                query = query.Where(c => c.MonHoc.ToLower() == searchParams.MonHoc.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(searchParams.ChuDe))
            {
                query = query.Where(c => c.ChuDe != null && c.ChuDe.ToLower() == searchParams.ChuDe.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(searchParams.DoKho))
            {
                query = query.Where(c => c.DoKho.ToLower() == searchParams.DoKho.ToLower());
            }

            // 2. Sắp xếp (Sorting)
            // Mặc định sắp xếp theo ngày tạo mới nhất
            if (string.IsNullOrWhiteSpace(searchParams.SortBy))
            {
                query = query.OrderByDescending(c => c.TaoLuc);
            }
            else
            {
                // Cú pháp sắp xếp động
                var parameter = Expression.Parameter(typeof(CauHoi), "x");
                var property = Expression.Property(parameter, searchParams.SortBy);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = (searchParams.SortDirection?.ToUpper() == "DESC")
                    ? "OrderByDescending"
                    : "OrderBy";

                var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(CauHoi), property.Type },
                    query.Expression, Expression.Quote(lambda));

                query = query.Provider.CreateQuery<CauHoi>(resultExpression);
            }

            // 3. Phân trang (Paging)
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .Include(c => c.LuaChons)
                .AsNoTracking()
                .ToListAsync();

            var dtos = _mapper.Map<List<CauHoiResponseDTO>>(items);
            var pagedResult = new PagedResult<CauHoiResponseDTO>(dtos, totalItems, searchParams.PageNumber, searchParams.PageSize);

            return ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThanhCongVoiDuLieu(pagedResult);
        }

        public async Task<ApiPhanHoi<ImportResultDTO>> ImportFromExcelAsync(IFormFile file, Guid teacherId)
        {
            if (file == null || file.Length == 0)
            {
                return ApiPhanHoi<ImportResultDTO>.ThatBai("Vui lòng chọn một file Excel để tải lên.");
            }

            var result = new ImportResultDTO();
            var newQuestions = new List<CauHoi>();

            // EPPlus cần có LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return ApiPhanHoi<ImportResultDTO>.ThatBai("File Excel không chứa worksheet nào.");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    result.TotalRows = rowCount - 1; // Trừ dòng header

                    // Giả sử file Excel có cấu trúc cột:
                    // A: NoiDung, B: MonHoc, C: DoKho, D: DapAnDung, E->H: LuaChon1->4, I: LaDapAnDung (chứa A,B,C,D)
                    for (int row = 2; row <= rowCount; row++) // Bắt đầu từ dòng 2, bỏ qua header
                    {
                        try
                        {
                            var noiDung = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var monHoc = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var doKho = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                            var dapAnDung = worksheet.Cells[row, 4].Value?.ToString()?.Trim();

                            // Validation cơ bản
                            if (string.IsNullOrWhiteSpace(noiDung) || string.IsNullOrWhiteSpace(monHoc) || string.IsNullOrWhiteSpace(dapAnDung))
                            {
                                result.FailedImports++;
                                result.ErrorMessages.Add($"Dòng {row}: Nội dung, Môn học, và Đáp án đúng là bắt buộc.");
                                continue;
                            }

                            var cauHoi = new CauHoi
                            {
                                Id = Guid.NewGuid(),
                                NoiDung = noiDung,
                                MonHoc = monHoc,
                                DoKho = string.IsNullOrWhiteSpace(doKho) ? "medium" : doKho,
                                DapAnDung = dapAnDung,
                                NguoiTaoId = teacherId,
                                LuaChons = new List<LuaChon>()
                            };

                            // Đọc các lựa chọn (giả sử có 4 lựa chọn từ cột E đến H)
                            for (int col = 5; col <= 8; col++)
                            {
                                var luaChonText = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(luaChonText))
                                {
                                    var choiceLetter = ((char)('A' + col - 5)).ToString();
                                    cauHoi.LuaChons.Add(new LuaChon
                                    {
                                        NoiDung = luaChonText,
                                        LaDapAnDung = dapAnDung.Equals(choiceLetter, StringComparison.OrdinalIgnoreCase)
                                    });
                                }
                            }

                            newQuestions.Add(cauHoi);
                            result.SuccessfulImports++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedImports++;
                            result.ErrorMessages.Add($"Dòng {row}: Lỗi xử lý dữ liệu - {ex.Message}");
                        }
                    }
                }
            }

            // Thêm tất cả câu hỏi hợp lệ vào DB trong một transaction
            if (newQuestions.Any())
            {
                foreach (var q in newQuestions)
                {
                    await _repo.CreateAsync(q);
                }
            }

            return ApiPhanHoi<ImportResultDTO>.ThanhCongVoiDuLieu(result, "Quá trình import hoàn tất.");
        }

        public async Task<byte[]> ExportToExcelAsync(Guid teacherId)
        {
            // Lấy tất cả câu hỏi của giáo viên
            var questions = await _repo.GetAllAsync(teacherId);

            // EPPlus cần có LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("NganHangCauHoi");

                // --- Định dạng Header ---
                var headerCells = worksheet.Cells["A1:I1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // --- Ghi Header ---
                worksheet.Cells[1, 1].Value = "NoiDung";
                worksheet.Cells[1, 2].Value = "MonHoc";
                worksheet.Cells[1, 3].Value = "DoKho";
                worksheet.Cells[1, 4].Value = "DapAnDung";
                worksheet.Cells[1, 5].Value = "LuaChonA";
                worksheet.Cells[1, 6].Value = "LuaChonB";
                worksheet.Cells[1, 7].Value = "LuaChonC";
                worksheet.Cells[1, 8].Value = "LuaChonD";
                worksheet.Cells[1, 9].Value = "GiaiThich";

                // --- Ghi Dữ liệu ---
                int row = 2;
                foreach (var q in questions)
                {
                    worksheet.Cells[row, 1].Value = q.NoiDung;
                    worksheet.Cells[row, 2].Value = q.MonHoc;
                    worksheet.Cells[row, 3].Value = q.DoKho;
                    worksheet.Cells[row, 4].Value = q.DapAnDung;

                    // Gán các lựa chọn vào các cột tương ứng
                    for (int i = 0; i < q.LuaChons.Count && i < 4; i++)
                    {
                        worksheet.Cells[row, 5 + i].Value = q.LuaChons.ElementAt(i).NoiDung;
                    }

                    worksheet.Cells[row, 9].Value = q.GiaiThich;
                    row++;
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Chuyển package thành mảng byte để trả về
                return await package.GetAsByteArrayAsync();
            }
        }



    }
}