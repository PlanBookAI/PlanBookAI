// ExamService/Controllers/ThongKeController.cs
using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/thong-ke")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;

        public ThongKeController(IThongKeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy báo cáo thống kê tổng quan về hoạt động của người dùng đang đăng nhập.
        /// </summary>
        [HttpGet("nguoi-dung")]
        public async Task<IActionResult> GetTeacherReport()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetTeacherReportAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Xuất báo cáo thống kê của giáo viên ra file Excel.
        /// </summary>
        [HttpGet("nguoi-dung/export-excel")]
        public async Task<IActionResult> ExportTeacherReport()
        {
            var teacherId = HttpContext.GetUserId();
            var fileBytes = await _service.ExportTeacherReportToExcelAsync(teacherId);

            if (fileBytes.Length == 0)
                return NotFound(ApiPhanHoi<object>.ThatBai("Không có dữ liệu để xuất báo cáo."));

            string fileName = $"BaoCaoThongKe_{teacherId}_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // === CÁC ENDPOINT CHI TIẾT ===
        // Các endpoint này sẽ gọi đến báo cáo chung và chỉ lấy phần dữ liệu cần thiết.

        [HttpGet("cau-hoi")]
        public async Task<IActionResult> GetQuestionStats()
        {
            var report = await _service.GetTeacherReportAsync(HttpContext.GetUserId());
            if (!report.ThanhCong) return BadRequest(report);
            return Ok(ApiPhanHoi<ThongKeTongQuanCauHoiDTO>.ThanhCongVoiDuLieu(report.DuLieu!.ThongKeCauHoi));
        }

        [HttpGet("de-thi")]
        public async Task<IActionResult> GetExamStats()
        {
            var report = await _service.GetTeacherReportAsync(HttpContext.GetUserId());
            if (!report.ThanhCong) return BadRequest(report);
            return Ok(ApiPhanHoi<ThongKeTongQuanDeThiDTO>.ThanhCongVoiDuLieu(report.DuLieu!.ThongKeDeThi));
        }

        [HttpGet("do-kho")]
        public async Task<IActionResult> GetStatsByDifficulty()
        {
            var report = await _service.GetTeacherReportAsync(HttpContext.GetUserId());
            if (!report.ThanhCong) return BadRequest(report);
            return Ok(ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(report.DuLieu!.ThongKeCauHoi.TheoDoKho));
        }

        [HttpGet("chu-de")]
        public async Task<IActionResult> GetStatsByTopic()
        {
            var report = await _service.GetTeacherReportAsync(HttpContext.GetUserId());
            if (!report.ThanhCong) return BadRequest(report);
            return Ok(ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(report.DuLieu!.ThongKeCauHoi.TheoChuDe));
        }

        [HttpGet("mon-hoc")]
        public async Task<IActionResult> GetStatsBySubject()
        {
            var report = await _service.GetTeacherReportAsync(HttpContext.GetUserId());
            if (!report.ThanhCong) return BadRequest(report);
            return Ok(ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(report.DuLieu!.ThongKeCauHoi.TheoMonHoc));
        }
    }
}