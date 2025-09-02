
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Models.DTOs;
using NotificationService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Controllers
{
    // Đánh dấu lớp này là một API Controller.
    [ApiController]
    // Định nghĩa base route cho tất cả các endpoint trong controller này.
    [Route("api/v1/thong-bao")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        // Sử dụng Dependency Injection để tiêm NotificationService.
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo của một người dùng.
        /// GET /api/v1/thong-bao/nguoi-dung/{userId}
        /// </summary>
        [HttpGet("nguoi-dung/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserId(string userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserId(userId);
            if (notifications == null)
            {
                return NotFound();
            }
            return Ok(notifications);
        }

        /// <summary>
        /// Lấy chi tiết một thông báo.
        /// GET /api/v1/thong-bao/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(Guid id)
        {
            var notification = await _notificationService.GetNotificationById(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        /// <summary>
        /// Tạo một thông báo mới.
        /// POST /api/v1/thong-bao
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] CreateNotificationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newNotification = await _notificationService.CreateNotification(createDto);
            return CreatedAtAction(nameof(GetNotificationById), new { id = newNotification.Id }, newNotification);
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// PUT /api/v1/thong-bao/{id}/doc
        /// </summary>
        [HttpPut("{id}/doc")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var success = await _notificationService.MarkAsRead(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Xóa một thông báo.
        /// DELETE /api/v1/thong-bao/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var success = await _notificationService.DeleteNotification(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
        public NotificationController(NotificationService.Services.NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
    }
}
