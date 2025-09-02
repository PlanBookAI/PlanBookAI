
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.DTOs;
using NotificationService.Services;
using System.Threading.Tasks;

namespace NotificationService.Controllers
{
    // Đánh dấu lớp này là một API Controller.
    [ApiController]
    // Định nghĩa base route cho tất cả các endpoint trong controller này.
    [Route("api/v1/email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        // Sử dụng Dependency Injection để tiêm EmailService.
        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Xử lý yêu cầu gửi email và đưa vào hàng đợi.
        /// POST /api/v1/email/gui
        /// </summary>
        [HttpPost("gui")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto sendDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailRecord = await _emailService.SendEmailAsync(sendDto);
            return Ok(new { Message = "Email đã được đưa vào hàng đợi để xử lý bất đồng bộ.", Id = emailRecord.Id });
        }
    }
}
