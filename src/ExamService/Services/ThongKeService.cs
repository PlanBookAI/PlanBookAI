using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly ICauHoiRepository _cauHoiRepo;
        private readonly IDeThiRepository _deThiRepo;

        public ThongKeService(ICauHoiRepository cauHoiRepo, IDeThiRepository deThiRepo)
        {
            _cauHoiRepo = cauHoiRepo;
            _deThiRepo = deThiRepo;
        }

        public async Task<ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>> GetTeacherReportAsync(Guid teacherId)
        {
            // Lấy IQueryable để xây dựng truy vấn mà không thực thi ngay
            var questionQuery = _cauHoiRepo.GetQueryable().Where(q => q.NguoiTaoId == teacherId);
            var examQuery = _deThiRepo.GetQueryable().Where(d => d.NguoiTaoId == teacherId);

            // === Chuẩn bị các task truy vấn cho Câu hỏi ===
            var questionCountTask = questionQuery.CountAsync();
            var qStatsBySubjectTask = questionQuery.GroupBy(q => q.MonHoc).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
            var qStatsByDifficultyTask = questionQuery.GroupBy(q => q.DoKho).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
            var qStatsByTopicTask = questionQuery.Where(q => q.ChuDe != null && q.ChuDe != "").GroupBy(q => q.ChuDe).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key!, SoLuong = g.Count() }).ToListAsync();

            // === Chuẩn bị các task truy vấn cho Đề thi ===
            var examCountTask = examQuery.CountAsync();
            var eStatsByStatusTask = examQuery.GroupBy(d => d.TrangThai).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
            var eStatsBySubjectTask = examQuery.GroupBy(d => d.MonHoc).Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() }).ToListAsync();
            var eStatsByGradeTask = examQuery.GroupBy(d => d.KhoiLop).Select(g => new ThongKeTheoNhomDTO { TenNhom = "Khối " + g.Key.ToString(), SoLuong = g.Count() }).ToListAsync();

            // Thực thi tất cả các task truy vấn một cách song song
            await Task.WhenAll(
                questionCountTask, qStatsBySubjectTask, qStatsByDifficultyTask, qStatsByTopicTask,
                examCountTask, eStatsByStatusTask, eStatsBySubjectTask, eStatsByGradeTask
            );

            // === Tổng hợp kết quả sau khi tất cả các task đã hoàn thành ===
            var report = new BaoCaoThongKeGiaoVienDTO
            {
                TeacherId = teacherId,
                ThongKeCauHoi = new ThongKeTongQuanCauHoiDTO
                {
                    TongSo = await questionCountTask,
                    TheoMonHoc = (await qStatsBySubjectTask).OrderBy(x => x.TenNhom).ToList(),
                    TheoDoKho = (await qStatsByDifficultyTask).OrderBy(x => x.TenNhom).ToList(),
                    TheoChuDe = (await qStatsByTopicTask).OrderBy(x => x.TenNhom).ToList()
                },
                ThongKeDeThi = new ThongKeTongQuanDeThiDTO
                {
                    TongSo = await examCountTask,
                    TheoTrangThai = (await eStatsByStatusTask).OrderBy(x => x.TenNhom).ToList(),
                    TheoMonHoc = (await eStatsBySubjectTask).OrderBy(x => x.TenNhom).ToList(),
                    TheoKhoiLop = (await eStatsByGradeTask).OrderBy(x => x.TenNhom).ToList()
                }
            };

            return ApiPhanHoi<BaoCaoThongKeGiaoVienDTO>.ThanhCongVoiDuLieu(report);
        }

        public async Task<byte[]> ExportTeacherReportToExcelAsync(Guid teacherId)
        {
            var reportResult = await GetTeacherReportAsync(teacherId);
            if (!reportResult.ThanhCong || reportResult.DuLieu == null)
            {
                return Array.Empty<byte>();
            }
            var data = reportResult.DuLieu;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            // --- Sheet 1: Thống kê Câu hỏi ---
            var wsQuestions = package.Workbook.Worksheets.Add("ThongKe_CauHoi");
            wsQuestions.Cells["A1"].Value = $"BÁO CÁO NGÂN HÀNG CÂU HỎI (Giáo viên: {data.TeacherId})";
            FormatHeader(wsQuestions.Cells["A1:D1"]);
            wsQuestions.Cells["A3"].Value = $"Tổng số câu hỏi: {data.ThongKeCauHoi.TongSo}";
            wsQuestions.Cells["A3"].Style.Font.Bold = true;

            int currentRow = 5;
            currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Môn học", data.ThongKeCauHoi.TheoMonHoc, "Môn học");
            currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Độ khó", data.ThongKeCauHoi.TheoDoKho, "Độ khó");
            currentRow = CreateStatsTable(wsQuestions, currentRow, "Thống kê theo Chủ đề", data.ThongKeCauHoi.TheoChuDe, "Chủ đề");
            wsQuestions.Cells[wsQuestions.Dimension.Address].AutoFitColumns();

            // --- Sheet 2: Thống kê Đề thi ---
            var wsExams = package.Workbook.Worksheets.Add("ThongKe_DeThi");
            wsExams.Cells["A1"].Value = $"BÁO CÁO ĐỀ THI (Giáo viên: {data.TeacherId})";
            FormatHeader(wsExams.Cells["A1:D1"]);
            wsExams.Cells["A3"].Value = $"Tổng số đề thi: {data.ThongKeDeThi.TongSo}";
            wsExams.Cells["A3"].Style.Font.Bold = true;

            currentRow = 5;
            currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Trạng thái", data.ThongKeDeThi.TheoTrangThai, "Trạng thái");
            currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Môn học", data.ThongKeDeThi.TheoMonHoc, "Môn học");
            currentRow = CreateStatsTable(wsExams, currentRow, "Thống kê theo Khối lớp", data.ThongKeDeThi.TheoKhoiLop, "Khối lớp");
            wsExams.Cells[wsExams.Dimension.Address].AutoFitColumns();

            return await package.GetAsByteArrayAsync();
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