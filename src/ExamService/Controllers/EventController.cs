using ExamService.Interfaces;
using ExamService.Models.DTOs; // Giả sử các DTOs sự kiện ở đây
using Microsoft.AspNetCore.Mvc;
using System;

namespace ExamService.Controllers
{
    // Đổi tên route cho phù hợp
    [ApiController]
    [Route("api/v1/event")]
    public class EventController : ControllerBase
    {
        private readonly IEventLogger _eventLogger;

        public EventController(IEventLogger eventLogger)
        {
            _eventLogger = eventLogger;
        }

        /// <summary>
        /// Ghi nhận sự kiện một đề thi mới đã được tạo.
        /// </summary>
        [HttpPost("de-thi-moi")]
        public IActionResult LogNewExamEvent([FromBody] NewExamEventDTO eventData)
        {
            _eventLogger.LogEvent("dethi.moi", eventData);
            return Accepted(new { Message = "Sự kiện de-thi-moi đã được ghi nhận." });
        }

        /// <summary>
        /// Ghi nhận sự kiện một câu hỏi mới đã được tạo.
        /// </summary>
        [HttpPost("cau-hoi-moi")]
        public IActionResult LogNewQuestionEvent([FromBody] NewQuestionEventDTO eventData)
        {
            _eventLogger.LogEvent("cauhoi.moi", eventData);
            return Accepted(new { Message = "Sự kiện cau-hoi-moi đã được ghi nhận." });
        }

        /// <summary>
        /// Ghi nhận sự kiện một đề thi đã được xuất bản.
        /// </summary>
        [HttpPost("xuat-ban-de-thi")]
        public IActionResult LogExamPublishedEvent([FromBody] ExamPublishedEventDTO eventData)
        {
            _eventLogger.LogEvent("dethi.xuatban", eventData);
            return Accepted(new { Message = "Sự kiện xuat-ban-de-thi đã được ghi nhận." });
        }
    }
}