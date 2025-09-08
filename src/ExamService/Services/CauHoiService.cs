using AutoMapper;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // Thêm using cho EPPlus
using OfficeOpenXml.Style; // Thêm using cho style
using System.Drawing; // Thêm using cho Color
using System.Linq.Expressions;
using MassTransit;
using ExamService.MessageContracts;

namespace ExamService.Services
{
    public class CauHoiService : ICauHoiService
    {
        private readonly ICauHoiRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint; // Inject IPublishEndpoint

        public CauHoiService(ICauHoiRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
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

            // Publish sự kiện sau khi tạo thành công
            await _publishEndpoint.Publish<CauHoiMoiCreated>(new
            {
                CauHoiId = newCauHoi.Id,
                NguoiTaoId = teacherId,
                MonHoc = newCauHoi.MonHoc,
                Timestamp = DateTime.UtcNow
            });

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

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    package.Compatibility.IsWorksheets1Based = false;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return ApiPhanHoi<ImportResultDTO>.ThatBai("File Excel không hợp lệ hoặc không có trang tính nào.");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    result.TotalRows = rowCount > 1 ? rowCount - 1 : 0;

                    for (int row = 2; row <= rowCount; row++) // Bắt đầu từ dòng 2, bỏ qua header
                    {
                        try
                        {
                            var noiDung = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var monHoc = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var doKho = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                            var dapAnDung = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                            var giaiThich = worksheet.Cells[row, 9].Value?.ToString()?.Trim();

                            // Validation dữ liệu từng dòng
                            if (string.IsNullOrWhiteSpace(noiDung) || string.IsNullOrWhiteSpace(monHoc) || string.IsNullOrWhiteSpace(dapAnDung))
                            {
                                result.FailedImports++;
                                result.ErrorMessages.Add($"Dòng {row}: Thiếu thông tin bắt buộc (Nội dung, Môn học, Đáp án đúng).");
                                continue;
                            }

                            var cauHoi = new CauHoi
                            {
                                Id = Guid.NewGuid(),
                                NoiDung = noiDung,
                                MonHoc = monHoc,
                                DoKho = string.IsNullOrWhiteSpace(doKho) ? "medium" : doKho,
                                DapAnDung = dapAnDung.ToUpper(),
                                GiaiThich = giaiThich,
                                NguoiTaoId = teacherId,
                                LuaChons = new List<LuaChon>()
                            };

                            // Đọc các lựa chọn và kiểm tra tính hợp lệ
                            int choiceCount = 0;
                            for (int col = 5; col <= 8; col++)
                            {
                                var luaChonText = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(luaChonText))
                                {
                                    choiceCount++;
                                    var choiceLetter = ((char)('A' + col - 5)).ToString();
                                    cauHoi.LuaChons.Add(new LuaChon
                                    {
                                        Id = Guid.NewGuid(),
                                        NoiDung = luaChonText,
                                        LaDapAnDung = dapAnDung.Equals(choiceLetter, StringComparison.OrdinalIgnoreCase),
                                        ThuTu = col - 4 // 1, 2, 3, 4
                                    });
                                }
                            }

                            if (choiceCount < 2)
                            {
                                result.FailedImports++;
                                result.ErrorMessages.Add($"Dòng {row}: Câu hỏi phải có ít nhất 2 lựa chọn.");
                                continue;
                            }

                            newQuestions.Add(cauHoi);
                            result.SuccessfulImports++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedImports++;
                            result.ErrorMessages.Add($"Dòng {row}: Lỗi không xác định - {ex.Message}");
                        }
                    }
                }
            }

            // Thêm tất cả câu hỏi hợp lệ vào DB
            if (newQuestions.Any())
            {
                // Sử dụng AddRange để tối ưu hóa việc thêm nhiều bản ghi
                await _repo.CreateRangeAsync(newQuestions);
            }

            return ApiPhanHoi<ImportResultDTO>.ThanhCongVoiDuLieu(result, "Quá trình import hoàn tất.");
        }

        public async Task<byte[]> ExportToExcelAsync(Guid teacherId)
        {
            var questions = await _repo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId)
                .Include(q => q.LuaChons)
                .OrderBy(q => q.MonHoc).ThenBy(q => q.ChuDe)
                .AsNoTracking()
                .ToListAsync();

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                package.Compatibility.IsWorksheets1Based = false;
                var worksheet = package.Workbook.Worksheets.Add("NganHangCauHoi");

                // --- Header ---
                string[] headers = { "NoiDung", "MonHoc", "DoKho", "DapAnDung", "LuaChonA", "LuaChonB", "LuaChonC", "LuaChonD", "GiaiThich" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#CCCCCC"));
                }

                // --- Dữ liệu ---
                int row = 2;
                foreach (var q in questions)
                {
                    worksheet.Cells[row, 1].Value = q.NoiDung;
                    worksheet.Cells[row, 2].Value = q.MonHoc;
                    worksheet.Cells[row, 3].Value = q.DoKho;
                    worksheet.Cells[row, 4].Value = q.DapAnDung;

                    var sortedChoices = q.LuaChons.OrderBy(c => c.ThuTu).ToList();
                    for (int i = 0; i < sortedChoices.Count && i < 4; i++)
                    {
                        worksheet.Cells[row, 5 + i].Value = sortedChoices[i].NoiDung;
                    }

                    worksheet.Cells[row, 9].Value = q.GiaiThich;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }



    }
}