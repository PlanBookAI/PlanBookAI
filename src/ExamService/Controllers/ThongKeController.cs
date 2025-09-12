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

        public ThongKeController(ILogger<ThongKeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lấy báo cáo thống kê tổng quan về hoạt động của người dùng đang đăng nhập.
        /// </summary>
        [HttpGet("nguoi-dung")]
        public IActionResult GetTeacherReport()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê người dùng");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        /// <summary>
        /// Xuất báo cáo thống kê của giáo viên ra file Excel.
        /// </summary>
        [HttpGet("nguoi-dung/export-excel")]
        public IActionResult ExportTeacherReport()
        {
            _logger.LogInformation("Đã nhận yêu cầu xuất Excel thống kê người dùng");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        // === CÁC ENDPOINT CHI TIẾT ===

        [HttpGet("cau-hoi")]
        public IActionResult GetQuestionStats()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê câu hỏi");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        [HttpGet("de-thi")]
        public IActionResult GetExamStats()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê đề thi");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        [HttpGet("do-kho")]
        public IActionResult GetStatsByDifficulty()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo độ khó");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        [HttpGet("chu-de")]
        public IActionResult GetStatsByTopic()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo chủ đề");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }

        [HttpGet("mon-hoc")]
        public IActionResult GetStatsBySubject()
        {
            _logger.LogInformation("Đã nhận yêu cầu thống kê theo môn học");
            return Ok(new ApiPhanHoi<object>
            {
                ThongBao = "Chức năng đang được hoàn thiện",
                DuLieu = new { TrangThai = "Đang phát triển" }
            });
        }
    }
}