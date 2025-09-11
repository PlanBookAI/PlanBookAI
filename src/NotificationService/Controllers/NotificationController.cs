using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Models.DTOs;
using NotificationService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/v1/thong-bao")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService.Services.NotificationService _notificationService;

        public NotificationController(NotificationService.Services.NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("nguoi-dung/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserId(Guid userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserId(userId);
            if (notifications == null)
            {
                return NotFound();
            }
            return Ok(notifications);
        }

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

        [HttpPost("gui-hang-loat")]
        public async Task<IActionResult> SendBulkNotification([FromBody] BulkNotificationRequestDto bulkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _notificationService.SendBulkNotification(bulkDto);
            return Ok();
        }

        [HttpPost("hen-gio")]
        public async Task<IActionResult> ScheduleNotification([FromBody] ScheduledNotificationRequestDto scheduledDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _notificationService.ScheduleNotification(scheduledDto);
            return Ok();
        }

        [HttpPut("{id}/huy-lich")]
        public async Task<IActionResult> CancelScheduledNotification(Guid id)
        {
            var success = await _notificationService.CancelScheduledNotification(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
