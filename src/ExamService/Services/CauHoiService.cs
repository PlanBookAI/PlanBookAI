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

            var result = new ImportResultDTO
            {
                TotalRows = 0,
                SuccessfulImports = 0,
                FailedImports = 0,
                ErrorMessages = new List<string>()
            };
            
            var newQuestions = new List<CauHoi>();

            try
            {
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

                        // Kiểm tra kích thước dữ liệu
                        if (worksheet.Dimension == null)
                        {
                            return ApiPhanHoi<ImportResultDTO>.ThatBai("File Excel trống hoặc không có dữ liệu.");
                        }
                        
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;
                        
                        // Kiểm tra số cột tối thiểu
                        if (colCount < 6)
                        {
                            return ApiPhanHoi<ImportResultDTO>.ThatBai("File Excel không đúng định dạng. Cần ít nhất 6 cột: NoiDung, MonHoc, DoKho, DapAnDung, LuaChonA, LuaChonB.");
                        }
                        
                        // Kiểm tra header
                        string[] expectedHeaders = { "NoiDung", "MonHoc", "DoKho", "DapAnDung", "LuaChonA", "LuaChonB", "LuaChonC", "LuaChonD", "GiaiThich" };
                        for (int i = 0; i < Math.Min(colCount, expectedHeaders.Length); i++)
                        {
                            string headerValue = worksheet.Cells[1, i + 1].Value?.ToString() ?? "";
                            if (!headerValue.Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                            {
                                return ApiPhanHoi<ImportResultDTO>.ThatBai($"Header không đúng định dạng. Cột {i + 1} nên là '{expectedHeaders[i]}' nhưng là '{headerValue}'.");
                            }
                        }

                        result.TotalRows = rowCount > 1 ? rowCount - 1 : 0;
                        
                        if (result.TotalRows == 0)
                        {
                            return ApiPhanHoi<ImportResultDTO>.ThatBai("File Excel không chứa dữ liệu câu hỏi (chỉ có header).");
                        }

                        for (int row = 2; row <= rowCount; row++) // Bắt đầu từ dòng 2, bỏ qua header
                        {
                            try
                            {
                                var noiDung = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                                var monHoc = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                                var doKho = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                                var dapAnDung = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                                var chuDe = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                                var giaiThich = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                                
                                // Chuẩn hóa các giá trị
                                monHoc = ChuanHoaMonHoc(monHoc ?? "HOA_HOC");
                                doKho = ChuanHoaDoKho(doKho ?? "MEDIUM");
                                dapAnDung = (dapAnDung ?? "").Trim().ToUpper();

                                // Validation dữ liệu từng dòng
                                if (string.IsNullOrWhiteSpace(noiDung))
                                {
                                    result.FailedImports++;
                                    result.ErrorMessages.Add($"Dòng {row}: Nội dung câu hỏi không được để trống.");
                                    continue;
                                }
                                
                                // Kiểm tra độ khó hợp lệ
                                if (!new[] { "EASY", "MEDIUM", "HARD", "VERY_HARD" }.Contains(doKho))
                                {
                                    result.FailedImports++;
                                    result.ErrorMessages.Add($"Dòng {row}: Độ khó '{doKho}' không hợp lệ. Phải là một trong: EASY, MEDIUM, HARD, VERY_HARD.");
                                    continue;
                                }
                                
                                // Kiểm tra đáp án đúng hợp lệ
                                if (string.IsNullOrEmpty(dapAnDung) || !new[] { "A", "B", "C", "D" }.Contains(dapAnDung))
                                {
                                    result.FailedImports++;
                                    result.ErrorMessages.Add($"Dòng {row}: Đáp án đúng '{dapAnDung}' không hợp lệ. Phải là một trong: A, B, C, D.");
                                    continue;
                                }

                                var cauHoi = new CauHoi
                                {
                                    Id = Guid.NewGuid(),
                                    NoiDung = noiDung,
                                    MonHoc = monHoc,
                                    DoKho = doKho,
                                    ChuDe = chuDe,
                                    DapAnDung = dapAnDung,
                                    GiaiThich = giaiThich,
                                    NguoiTaoId = teacherId,
                                    LoaiCauHoi = "MULTIPLE_CHOICE", // Mặc định
                                    TrangThai = "ACTIVE",
                                    LuaChons = new List<LuaChon>(),
                                    TaoLuc = DateTime.UtcNow,
                                    CapNhatLuc = DateTime.UtcNow
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
                                            CauHoiId = cauHoi.Id,
                                            MaLuaChon = choiceLetter,
                                            TaoLuc = DateTime.UtcNow
                                        });
                                    }
                                }

                                if (choiceCount < 2)
                                {
                                    result.FailedImports++;
                                    result.ErrorMessages.Add($"Dòng {row}: Câu hỏi phải có ít nhất 2 lựa chọn.");
                                    continue;
                                }
                                
                                // Kiểm tra đáp án đúng có trong các lựa chọn
                                if (!cauHoi.LuaChons.Any(lc => lc.MaLuaChon == dapAnDung))
                                {
                                    result.FailedImports++;
                                    result.ErrorMessages.Add($"Dòng {row}: Đáp án đúng '{dapAnDung}' không tồn tại trong các lựa chọn.");
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
                    try
                    {
                        // Sử dụng AddRange để tối ưu hóa việc thêm nhiều bản ghi
                        await _repo.CreateRangeAsync(newQuestions);
                        
                        // Gửi thông báo qua RabbitMQ nếu có nhiều câu hỏi được thêm
                        if (newQuestions.Count > 5)
                        {
                            try
                            {
                                await _publishEndpoint.Publish<CauHoiImported>(new 
                                {
                                    TeacherId = teacherId,
                                    NumberOfQuestions = newQuestions.Count,
                                    ImportedAt = DateTime.UtcNow
                                });
                            }
                            catch (Exception mqEx)
                            {
                                // Lỗi RabbitMQ không ảnh hưởng đến kết quả import
                                // Chỉ log lỗi
                                Console.WriteLine($"Lỗi khi gửi thông báo RabbitMQ: {mqEx.Message}");
                            }
                        }
                    }
                    catch (Exception dbEx)
                    {
                        // Lỗi khi lưu vào DB
                        return ApiPhanHoi<ImportResultDTO>.ThatBai($"Lỗi khi lưu câu hỏi vào cơ sở dữ liệu: {dbEx.Message}");
                    }
                }

                return ApiPhanHoi<ImportResultDTO>.ThanhCongVoiDuLieu(result, $"Quá trình import hoàn tất. Đã import thành công {result.SuccessfulImports}/{result.TotalRows} câu hỏi.");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ImportResultDTO>.ThatBai($"Lỗi khi xử lý file Excel: {ex.Message}");
            }
        }
        
        private string ChuanHoaMonHoc(string monHoc)
        {
            monHoc = monHoc.Trim().ToUpper();
            
            // Ánh xạ các tên môn học phổ biến
            var monHocMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HOA", "HOA_HOC" },
                { "HOAHOC", "HOA_HOC" },
                { "HOA HOC", "HOA_HOC" },
                { "HÓA HỌC", "HOA_HOC" },
                { "TOAN", "TOAN_HOC" },
                { "TOANHOC", "TOAN_HOC" },
                { "TOAN HOC", "TOAN_HOC" },
                { "TOÁN HỌC", "TOAN_HOC" },
                { "LY", "VAT_LY" },
                { "VATLY", "VAT_LY" },
                { "VAT LY", "VAT_LY" },
                { "VẬT LÝ", "VAT_LY" },
                { "SINH", "SINH_HOC" },
                { "SINHHOC", "SINH_HOC" },
                { "SINH HOC", "SINH_HOC" },
                { "SINH HỌC", "SINH_HOC" }
            };
            
            if (monHocMapping.ContainsKey(monHoc))
            {
                return monHocMapping[monHoc];
            }
            
            return monHoc;
        }
        
        private string ChuanHoaDoKho(string doKho)
        {
            doKho = doKho.Trim().ToUpper();
            
            // Ánh xạ các mức độ khó
            var doKhoMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "DE", "EASY" },
                { "DỄ", "EASY" },
                { "TRUNG BINH", "MEDIUM" },
                { "TRUNG BÌNH", "MEDIUM" },
                { "TB", "MEDIUM" },
                { "KHO", "HARD" },
                { "KHÓ", "HARD" },
                { "RAT KHO", "VERY_HARD" },
                { "RẤT KHÓ", "VERY_HARD" }
            };
            
            if (doKhoMapping.ContainsKey(doKho))
            {
                return doKhoMapping[doKho];
            }
            
            return doKho;
        }

        public async Task<byte[]> ExportToExcelAsync(Guid teacherId)
        {
            try
            {
                var questions = await _repo.GetQueryable()
                    .Where(q => q.NguoiTaoId == teacherId)
                    .Include(q => q.LuaChons)
                    .OrderBy(q => q.MonHoc)
                    .ThenBy(q => q.ChuDe)
                    .AsNoTracking()
                    .ToListAsync();

                if (questions == null || !questions.Any())
                {
                    // Tạo file Excel trống với thông báo
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("NganHangCauHoi");
                        worksheet.Cells[1, 1].Value = "Không có câu hỏi nào trong ngân hàng câu hỏi của bạn.";
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        return await package.GetAsByteArrayAsync();
                    }
                }

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    package.Compatibility.IsWorksheets1Based = false;
                    var worksheet = package.Workbook.Worksheets.Add("NganHangCauHoi");

                    // Thêm thông tin header
                    worksheet.Cells[1, 1, 1, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "NGÂN HÀNG CÂU HỎI";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 14;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    // Thêm thông tin thống kê
                    worksheet.Cells[2, 1, 2, 9].Merge = true;
                    worksheet.Cells[2, 1].Value = $"Tổng số câu hỏi: {questions.Count} | Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    worksheet.Cells[2, 1].Style.Font.Italic = true;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // --- Header ---
                    string[] headers = { "NoiDung", "MonHoc", "DoKho", "DapAnDung", "LuaChonA", "LuaChonB", "LuaChonC", "LuaChonD", "GiaiThich" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[4, i + 1].Value = headers[i];
                    }
                    
                    using (var range = worksheet.Cells[4, 1, 4, headers.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#CCCCCC"));
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    // --- Dữ liệu ---
                    int row = 5; // Bắt đầu từ dòng 5 (sau header)
                    foreach (var q in questions)
                    {
                        worksheet.Cells[row, 1].Value = q.NoiDung;
                        worksheet.Cells[row, 2].Value = q.MonHoc;
                        worksheet.Cells[row, 3].Value = q.DoKho;
                        worksheet.Cells[row, 4].Value = q.DapAnDung;

                        var sortedChoices = q.LuaChons.OrderBy(c => c.MaLuaChon).ToList();
                        for (int i = 0; i < sortedChoices.Count && i < 4; i++)
                        {
                            worksheet.Cells[row, 5 + i].Value = sortedChoices[i].NoiDung;
                        }

                        worksheet.Cells[row, 9].Value = q.GiaiThich;
                        
                        // Định dạng dòng
                        using (var rowRange = worksheet.Cells[row, 1, row, headers.Length])
                        {
                            rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            
                            // Đổi màu nền cho dòng chẵn
                            if (row % 2 == 0)
                            {
                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F2F2F2"));
                            }
                        }
                        
                        row++;
                    }

                    // Tự động điều chỉnh độ rộng cột
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    
                    // Đặt độ rộng tối đa cho cột nội dung
                    worksheet.Column(1).Width = Math.Min(worksheet.Column(1).Width, 50);
                    
                    // Đặt wrap text cho các cột dài
                    worksheet.Column(1).Style.WrapText = true; // Nội dung
                    worksheet.Column(9).Style.WrapText = true; // Giải thích
                    
                    // Đặt filter cho header
                    worksheet.Cells[4, 1, 4, headers.Length].AutoFilter = true;
                    
                    // Đóng băng hàng đầu tiên
                    worksheet.View.FreezePanes(5, 1);

                    try
                    {
                        // Gửi thông báo qua RabbitMQ
                        await _publishEndpoint.Publish<CauHoiExported>(new
                        {
                            TeacherId = teacherId,
                            NumberOfQuestions = questions.Count,
                            ExportedAt = DateTime.UtcNow
                        });
                    }
                    catch (Exception mqEx)
                    {
                        // Lỗi RabbitMQ không ảnh hưởng đến kết quả export
                        // Chỉ log lỗi
                        Console.WriteLine($"Lỗi khi gửi thông báo RabbitMQ: {mqEx.Message}");
                    }

                    return await package.GetAsByteArrayAsync();
                }
            }
            catch (Exception ex)
            {
                // Tạo file Excel với thông báo lỗi
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Lỗi");
                    worksheet.Cells[1, 1].Value = "Đã xảy ra lỗi khi xuất dữ liệu:";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[2, 1].Value = ex.Message;
                    return await package.GetAsByteArrayAsync();
                }
            }
        }



    }
}