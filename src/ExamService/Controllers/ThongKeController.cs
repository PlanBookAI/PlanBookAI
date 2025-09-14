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
        private readonly ILogger<ThongKeController> _logger;
        private readonly IThongKeService _thongKeService;

        public ThongKeController(ILogger<ThongKeController> logger, IThongKeService thongKeService)
        {
            _logger = logger;
            _thongKeService = thongKeService;
        }

        /// <summary>
        /// Lấy báo cáo thống kê tổng quan về hoạt động của người dùng đang đăng nhập.
        /// </summary>
        [HttpGet("nguoi-dung")]
        public async Task<IActionResult> GetTeacherReport()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê người dùng");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetTeacherReportAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Xuất báo cáo thống kê của giáo viên ra file Excel.
        /// </summary>
        [HttpGet("nguoi-dung/export-excel")]
        public async Task<IActionResult> ExportTeacherReport()
        {
            _logger.LogInformation("Đã nhận yêu cầu xuất Excel thống kê người dùng");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.ExportTeacherReportAsync(teacherId);
            
            if (result.MaTrangThai != 200)
            {
                return BadRequest(result);
            }
            
            return File(result.DuLieu, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "thong-ke-giao-vien.xlsx");
        }

        // === CÁC ENDPOINT CHI TIẾT ===

        [HttpGet("cau-hoi")]
        public async Task<IActionResult> GetQuestionStats()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê câu hỏi");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetQuestionStatsAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("de-thi")]
        public async Task<IActionResult> GetExamStats()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê đề thi");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetExamStatsAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("do-kho")]
        public async Task<IActionResult> GetStatsByDifficulty()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo độ khó");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetStatsByDifficultyAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("chu-de")]
        public async Task<IActionResult> GetStatsByTopic()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo chủ đề");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetStatsByTopicAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("mon-hoc")]
        public async Task<IActionResult> GetStatsBySubject()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo môn học");
            var teacherId = HttpContext.GetUserId();
            var result = await _thongKeService.GetStatsBySubjectAsync(teacherId);
            return Ok(result);
        }
    }
}