using ExamService.Interfaces;
using ExamService.MessageContracts;
using ExamService.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace ExamService.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly ICauHoiRepository _cauHoiRepo;
        private readonly IDeThiRepository _deThiRepo;
        private readonly IPublishEndpoint _publishEndpoint;

        public ThongKeService(
            ICauHoiRepository cauHoiRepo, 
            IDeThiRepository deThiRepo,
            IPublishEndpoint publishEndpoint)
        {
            _cauHoiRepo = cauHoiRepo;
            _deThiRepo = deThiRepo;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>> GetTeacherReportAsync(Guid teacherId)
        {
            try
            {
                // Lấy IQueryable để xây dựng truy vấn mà không thực thi ngay
                var questionQuery = _cauHoiRepo.GetQueryable().Where(q => q.NguoiTaoId == teacherId);
                var examQuery = _deThiRepo.GetQueryable().Where(d => d.NguoiTaoId == teacherId);

                // === Chuẩn bị các task truy vấn cho Câu hỏi ===
                var questionCountTask = questionQuery.CountAsync();
                var qStatsBySubjectTask = questionQuery.GroupBy(q => q.MonHoc).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
                var qStatsByDifficultyTask = questionQuery.GroupBy(q => q.DoKho).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
                var qStatsByTopicTask = questionQuery.Where(q => q.ChuDe != null && q.ChuDe != "").GroupBy(q => q.ChuDe).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key!, SoLuong = g.Count() }).ToListAsync();
                var qStatsByTypeTask = questionQuery.GroupBy(q => q.LoaiCauHoi).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();

                // === Chuẩn bị các task truy vấn cho Đề thi ===
                var examCountTask = examQuery.CountAsync();
                var eStatsByStatusTask = examQuery.GroupBy(d => d.TrangThai).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
                var eStatsBySubjectTask = examQuery.GroupBy(d => d.MonHoc).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
                var eStatsByGradeTask = examQuery.GroupBy(d => d.KhoiLop).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key.HasValue ? $"Khối {g.Key}" : "Không xác định", SoLuong = g.Count() }).ToListAsync();
                
                // Thống kê phân phối số lượng câu hỏi trong đề thi
                var examQuestionCountsTask = examQuery
                    .Include(d => d.ExamQuestions)
                    .Select(d => new { d.Id, QuestionCount = d.ExamQuestions.Count })
                    .ToListAsync();

                // Thực thi tất cả các task truy vấn một cách song song
                await Task.WhenAll(
                    questionCountTask, qStatsBySubjectTask, qStatsByDifficultyTask, qStatsByTopicTask, qStatsByTypeTask,
                    examCountTask, eStatsByStatusTask, eStatsBySubjectTask, eStatsByGradeTask, examQuestionCountsTask
                );

                // Tính toán phân phối số lượng câu hỏi trong đề thi
                var examQuestionCounts = await examQuestionCountsTask;
                var questionCountDistribution = new ThongKePhanPhoi();
                
                if (examQuestionCounts.Any())
                {
                    questionCountDistribution.Min = examQuestionCounts.Min(e => e.QuestionCount);
                    questionCountDistribution.Max = examQuestionCounts.Max(e => e.QuestionCount);
                    questionCountDistribution.TrungBinh = examQuestionCounts.Average(e => e.QuestionCount);
                    
                    // Tính trung vị
                    var sortedCounts = examQuestionCounts.Select(e => e.QuestionCount).OrderBy(c => c).ToList();
                    int midPoint = sortedCounts.Count / 2;
                    questionCountDistribution.TrungVi = sortedCounts.Count % 2 == 0
                        ? (sortedCounts[midPoint - 1] + sortedCounts[midPoint]) / 2.0
                        : sortedCounts[midPoint];
                    
                    // Tạo phân phối
                    var distribution = examQuestionCounts
                        .GroupBy(e => e.QuestionCount)
                        .Select(g => new ThongKeTheoNhomDTO 
                        { 
                            TenNhom = g.Key.ToString() + " câu hỏi", 
                            SoLuong = g.Count(),
                            TyLePhanTram = Math.Round(100.0 * g.Count() / examQuestionCounts.Count, 1)
                        })
                        .OrderBy(x => int.Parse(x.TenNhom.Split(' ')[0]))
                        .ToList();
                    
                    questionCountDistribution.PhanPhoi = distribution;
                }

                // Tính tỷ lệ phần trăm cho các thống kê
                var totalQuestions = await questionCountTask;
                if (totalQuestions > 0)
                {
                    foreach (var item in await qStatsBySubjectTask)
                    {
                        item.TyLePhanTram = Math.Round(100.0 * item.SoLuong / totalQuestions, 1);
                    }
                    
                    foreach (var item in await qStatsByDifficultyTask)
                    {
                        item.TyLePhanTram = Math.Round(100.0 * item.SoLuong / totalQuestions, 1);
                    }
                    
                    foreach (var item in await qStatsByTypeTask)
                    {
                        item.TyLePhanTram = Math.Round(100.0 * item.SoLuong / totalQuestions, 1);
                    }
                }
                
                var totalExams = await examCountTask;
                if (totalExams > 0)
                {
                    foreach (var item in await eStatsByStatusTask)
                    {
                        item.TyLePhanTram = Math.Round(100.0 * item.SoLuong / totalExams, 1);
                    }
                    
                    foreach (var item in await eStatsBySubjectTask)
                    {
                        item.TyLePhanTram = Math.Round(100.0 * item.SoLuong / totalExams, 1);
                    }
                }

                // === Tổng hợp kết quả sau khi tất cả các task đã hoàn thành ===
                var report = new BaoCaoThongKeGiaoVienDTO
                {
                    TeacherId = teacherId,
                    ThoiDiemBaoCao = DateTime.UtcNow,
                    ThongKeCauHoi = new ThongKeTongQuanCauHoiDTO
                    {
                        TongSo = await questionCountTask,
                        TheoMonHoc = (await qStatsBySubjectTask).OrderByDescending(x => x.SoLuong).ToList(),
                        TheoDoKho = (await qStatsByDifficultyTask).OrderBy(x => x.TenNhom).ToList(),
                        TheoChuDe = (await qStatsByTopicTask).OrderByDescending(x => x.SoLuong).ToList(),
                        TheoLoai = (await qStatsByTypeTask).OrderByDescending(x => x.SoLuong).ToList()
                    },
                    ThongKeDeThi = new ThongKeTongQuanDeThiDTO
                    {
                        TongSo = await examCountTask,
                        TheoTrangThai = (await eStatsByStatusTask).OrderBy(x => x.TenNhom).ToList(),
                        TheoMonHoc = (await eStatsBySubjectTask).OrderByDescending(x => x.SoLuong).ToList(),
                        TheoKhoiLop = (await eStatsByGradeTask).OrderBy(x => x.TenNhom).ToList(),
                        TheoSoLuongCauHoi = questionCountDistribution
                    },
                    ThongKeHoatDong = new ThongKeHoatDongDTO
                    {
                        HoatDongGanNhat = DateTime.UtcNow,
                        TheoNgay = new List<ThongKeTheoNgayDTO>(),
                        TheoLoaiHoatDong = new List<ThongKeTheoNhomDTO>()
                    }
                };

                // Gửi thông báo qua RabbitMQ
                try
                {
                    await _publishEndpoint.Publish<ThongKeGenerated>(new
                    {
                        TeacherId = teacherId,
                        GeneratedAt = DateTime.UtcNow,
                        ReportType = "TeacherReport"
                    });
                }
                catch (Exception mqEx)
                {
                    // Lỗi RabbitMQ không ảnh hưởng đến kết quả thống kê
                    Console.WriteLine($"Lỗi khi gửi thông báo RabbitMQ: {mqEx.Message}");
                }

                return ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>.ThanhCongVoiDuLieu(report);
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>.ThatBai($"Lỗi khi tạo báo cáo thống kê: {ex.Message}");
            }
        }

        public async Task<byte[]> ExportTeacherReportToExcelAsync(Guid teacherId)
        {
            try
            {
                var reportResult = await GetTeacherReportAsync(teacherId);
                if (reportResult.MaTrangThai != 200 || reportResult.DuLieu == null)
                {
                    // Tạo file Excel với thông báo lỗi
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Lỗi");
                        worksheet.Cells["A1"].Value = "Không thể tạo báo cáo thống kê";
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Value = reportResult.ThongBao;
                        return await package.GetAsByteArrayAsync();
                    }
                }
                
                var data = reportResult.DuLieu;

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    // --- Trang bìa ---
                    var wsCover = package.Workbook.Worksheets.Add("TrangBia");
                    wsCover.Cells["A1:F1"].Merge = true;
                    wsCover.Cells["A1"].Value = "BÁO CÁO THỐNG KÊ PLANBOOKAI";
                    wsCover.Cells["A1"].Style.Font.Bold = true;
                    wsCover.Cells["A1"].Style.Font.Size = 20;
                    wsCover.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    wsCover.Cells["A3:F3"].Merge = true;
                    wsCover.Cells["A3"].Value = $"Giáo viên: {data.TeacherId}";
                    wsCover.Cells["A3"].Style.Font.Bold = true;
                    wsCover.Cells["A3"].Style.Font.Size = 14;
                    
                    wsCover.Cells["A4:F4"].Merge = true;
                    wsCover.Cells["A4"].Value = $"Thời điểm báo cáo: {data.ThoiDiemBaoCao:dd/MM/yyyy HH:mm}";
                    wsCover.Cells["A4"].Style.Font.Italic = true;
                    
                    wsCover.Cells["A6:F6"].Merge = true;
                    wsCover.Cells["A6"].Value = "Tổng quan";
                    wsCover.Cells["A6"].Style.Font.Bold = true;
                    wsCover.Cells["A6"].Style.Font.Size = 14;
                    
                    wsCover.Cells["A8"].Value = "Tổng số câu hỏi:";
                    wsCover.Cells["B8"].Value = data.ThongKeCauHoi.TongSo;
                    wsCover.Cells["A8:B8"].Style.Font.Bold = true;
                    
                    wsCover.Cells["A9"].Value = "Tổng số đề thi:";
                    wsCover.Cells["B9"].Value = data.ThongKeDeThi.TongSo;
                    wsCover.Cells["A9:B9"].Style.Font.Bold = true;
                    
                    wsCover.Cells["A11:F11"].Merge = true;
                    wsCover.Cells["A11"].Value = "Báo cáo này bao gồm các thống kê chi tiết về:";
                    wsCover.Cells["A11"].Style.Font.Bold = true;
                    
                    wsCover.Cells["A13"].Value = "- Thống kê câu hỏi theo môn học, độ khó, chủ đề và loại";
                    wsCover.Cells["A14"].Value = "- Thống kê đề thi theo trạng thái, môn học và khối lớp";
                    wsCover.Cells["A15"].Value = "- Phân phối số lượng câu hỏi trong đề thi";
                    
                    wsCover.Cells[wsCover.Dimension.Address].AutoFitColumns();

                    // --- Sheet 1: Thống kê Câu hỏi ---
                    var wsQuestions = package.Workbook.Worksheets.Add("ThongKe_CauHoi");
                    wsQuestions.Cells["A1:F1"].Merge = true;
                    wsQuestions.Cells["A1"].Value = "BÁO CÁO NGÂN HÀNG CÂU HỎI";
                    wsQuestions.Cells["A1"].Style.Font.Bold = true;
                    wsQuestions.Cells["A1"].Style.Font.Size = 16;
                    wsQuestions.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    wsQuestions.Cells["A3:B3"].Merge = true;
                    wsQuestions.Cells["A3"].Value = $"Tổng số câu hỏi: {data.ThongKeCauHoi.TongSo}";
                    wsQuestions.Cells["A3"].Style.Font.Bold = true;

                    int currentRow = 5;
                    currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Môn học", data.ThongKeCauHoi.TheoMonHoc, "Môn học");
                    
                    // Thêm biểu đồ thống kê theo môn học
                    if (data.ThongKeCauHoi.TheoMonHoc.Any())
                    {
                        var chartMonHoc = wsQuestions.Drawings.AddChart("ChartMonHoc", eChartType.Pie);
                        chartMonHoc.SetPosition(currentRow - 10, 0, 6, 0);
                        chartMonHoc.SetSize(400, 300);
                        
                        var seriesMonHoc = chartMonHoc.Series.Add(
                            ExcelRange.GetAddress(currentRow - data.ThongKeCauHoi.TheoMonHoc.Count, 2, currentRow - 1, 2),
                            ExcelRange.GetAddress(currentRow - data.ThongKeCauHoi.TheoMonHoc.Count, 1, currentRow - 1, 1)
                        );
                        chartMonHoc.Title.Text = "Phân bố câu hỏi theo môn học";
                        seriesMonHoc.Header = "Số lượng";
                        // Thiết lập hiển thị nhãn dữ liệu
                        // Lưu ý: Phiên bản EPPlus hiện tại có thể không hỗ trợ DataLabel
                        // Bỏ qua thiết lập DataLabel
                        chartMonHoc.Legend.Position = eLegendPosition.Bottom;
                        
                        currentRow += 15; // Thêm khoảng trống cho biểu đồ
                    }
                    
                    currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Độ khó", data.ThongKeCauHoi.TheoDoKho, "Độ khó");
                    currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Chủ đề", data.ThongKeCauHoi.TheoChuDe, "Chủ đề");
                    currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Loại câu hỏi", data.ThongKeCauHoi.TheoLoai, "Loại câu hỏi");
                    wsQuestions.Cells[wsQuestions.Dimension.Address].AutoFitColumns();

                    // --- Sheet 2: Thống kê Đề thi ---
                    var wsExams = package.Workbook.Worksheets.Add("ThongKe_DeThi");
                    wsExams.Cells["A1:F1"].Merge = true;
                    wsExams.Cells["A1"].Value = "BÁO CÁO THỐNG KÊ ĐỀ THI";
                    wsExams.Cells["A1"].Style.Font.Bold = true;
                    wsExams.Cells["A1"].Style.Font.Size = 16;
                    wsExams.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    wsExams.Cells["A3:B3"].Merge = true;
                    wsExams.Cells["A3"].Value = $"Tổng số đề thi: {data.ThongKeDeThi.TongSo}";
                    wsExams.Cells["A3"].Style.Font.Bold = true;

                    currentRow = 5;
                    currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Trạng thái", data.ThongKeDeThi.TheoTrangThai, "Trạng thái");
                    currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Môn học", data.ThongKeDeThi.TheoMonHoc, "Môn học");
                    currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Khối lớp", data.ThongKeDeThi.TheoKhoiLop, "Khối lớp");
                    
                    // Thêm thống kê phân phối số lượng câu hỏi
                    if (data.ThongKeDeThi.TheoSoLuongCauHoi.PhanPhoi.Any())
                    {
                        wsExams.Cells[currentRow, 1].Value = "Phân phối số lượng câu hỏi trong đề thi";
                        wsExams.Cells[currentRow, 1, currentRow, 3].Merge = true;
                        wsExams.Cells[currentRow, 1].Style.Font.Bold = true;
                        wsExams.Cells[currentRow, 1].Style.Font.Size = 12;
                        currentRow++;
                        
                        wsExams.Cells[currentRow, 1].Value = "Giá trị nhỏ nhất:";
                        wsExams.Cells[currentRow, 2].Value = data.ThongKeDeThi.TheoSoLuongCauHoi.Min;
                        currentRow++;
                        
                        wsExams.Cells[currentRow, 1].Value = "Giá trị lớn nhất:";
                        wsExams.Cells[currentRow, 2].Value = data.ThongKeDeThi.TheoSoLuongCauHoi.Max;
                        currentRow++;
                        
                        wsExams.Cells[currentRow, 1].Value = "Giá trị trung bình:";
                        wsExams.Cells[currentRow, 2].Value = data.ThongKeDeThi.TheoSoLuongCauHoi.TrungBinh;
                        currentRow++;
                        
                        wsExams.Cells[currentRow, 1].Value = "Giá trị trung vị:";
                        wsExams.Cells[currentRow, 2].Value = data.ThongKeDeThi.TheoSoLuongCauHoi.TrungVi;
                        currentRow += 2;
                        
                        // Tạo bảng phân phối
                        wsExams.Cells[currentRow, 1].Value = "Số câu hỏi";
                        wsExams.Cells[currentRow, 2].Value = "Số lượng đề thi";
                        wsExams.Cells[currentRow, 3].Value = "Tỷ lệ (%)";
                        
                        var headerRange = wsExams.Cells[currentRow, 1, currentRow, 3];
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        currentRow++;
                        
                        foreach (var item in data.ThongKeDeThi.TheoSoLuongCauHoi.PhanPhoi)
                        {
                            wsExams.Cells[currentRow, 1].Value = item.TenNhom;
                            wsExams.Cells[currentRow, 2].Value = item.SoLuong;
                            wsExams.Cells[currentRow, 3].Value = item.TyLePhanTram;
                            currentRow++;
                        }
                        
                        // Thêm biểu đồ cột cho phân phối
                        var chartDist = wsExams.Drawings.AddChart("ChartDistribution", eChartType.ColumnClustered);
                        chartDist.SetPosition(currentRow + 1, 0, 0, 0);
                        chartDist.SetSize(500, 300);
                        
                        var startRow = currentRow - data.ThongKeDeThi.TheoSoLuongCauHoi.PhanPhoi.Count;
                        var endRow = currentRow - 1;
                        
                        var seriesDist = chartDist.Series.Add(
                            ExcelRange.GetAddress(startRow, 2, endRow, 2),
                            ExcelRange.GetAddress(startRow, 1, endRow, 1)
                        );
                        
                        chartDist.Title.Text = "Phân phối số lượng câu hỏi trong đề thi";
                        seriesDist.Header = "Số lượng đề thi";
                        chartDist.XAxis.Title.Text = "Số câu hỏi";
                        chartDist.YAxis.Title.Text = "Số lượng đề thi";
                        // Bỏ qua thiết lập DataLabel
                        
                        currentRow += 18; // Thêm khoảng trống cho biểu đồ
                    }
                    
                    wsExams.Cells[wsExams.Dimension.Address].AutoFitColumns();
                    
                    // Gửi thông báo qua RabbitMQ
                    try
                    {
                    await _publishEndpoint.Publish<ThongKeExported>(new
                    {
                        TeacherId = teacherId,
                        ExportedAt = DateTime.UtcNow,
                        ReportType = "ExcelReport"
                    });
                    }
                    catch (Exception mqEx)
                    {
                        // Lỗi RabbitMQ không ảnh hưởng đến kết quả export
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
                    worksheet.Cells["A1"].Value = "Đã xảy ra lỗi khi tạo báo cáo Excel:";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A2"].Value = ex.Message;
                    return await package.GetAsByteArrayAsync();
                }
            }
        }

        // --- Hàm helper để định dạng và tạo bảng trong Excel ---
        private void FormatHeader(ExcelRange range)
        {
            range.Merge = true;
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 16;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private int CreateStatsTable(ExcelWorksheet worksheet, int startRow, string title, List<ThongKeTheoNhomDTO> data, string groupName)
        {
            // Title
            worksheet.Cells[startRow, 1].Value = title;
            worksheet.Cells[startRow, 1, startRow, 2].Merge = true;
            worksheet.Cells[startRow, 1, startRow, 2].Style.Font.Bold = true;
            worksheet.Cells[startRow, 1, startRow, 2].Style.Font.Size = 12;
            startRow++;

            // Table Header
            worksheet.Cells[startRow, 1].Value = groupName;
            worksheet.Cells[startRow, 2].Value = "Số lượng";
            var headerRange = worksheet.Cells[startRow, 1, startRow, 2];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            startRow++;

            // Table Data
            if (data.Any())
            {
                foreach (var item in data)
                {
                    worksheet.Cells[startRow, 1].Value = item.TenNhom;
                    worksheet.Cells[startRow, 2].Value = item.SoLuong;
                    startRow++;
                }
            }
            else
            {
                worksheet.Cells[startRow, 1].Value = "Không có dữ liệu.";
                worksheet.Cells[startRow, 1, startRow, 2].Merge = true;
                startRow++;
            }

            // Return next start row
            return startRow + 2; // Add 2 empty rows for spacing
        }
    }
}