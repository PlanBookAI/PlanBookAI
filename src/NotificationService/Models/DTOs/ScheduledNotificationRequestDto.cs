using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTOs
{
    /// <summary>
    /// Lớp DTO cho yêu cầu lên lịch gửi thông báo.
    /// </summary>
    public class ScheduledNotificationRequestDto
    {
        /// <summary>
        /// ID của người dùng nhận thông báo.
        /// </summary>
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        /// <summary>
        /// ID của template thông báo sẽ được sử dụng.
        /// </summary>
        [Required(ErrorMessage = "TemplateId is required.")]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Thời gian lên lịch gửi thông báo (định dạng UTC).
        /// </summary>
        [Required(ErrorMessage = "Scheduled time is required.")]
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// Dữ liệu biến cho template ở định dạng JSON.
        /// </summary>
        public object Variables { get; set; }
    }
}
