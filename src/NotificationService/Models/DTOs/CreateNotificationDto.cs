
using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTOs
{

    public class CreateNotificationDto
    {
        [Required]
        // ID của người dùng nhận thông báo.
        public string UserId { get; set; }

        [Required]
        // Tiêu đề của thông báo.
        public string Title { get; set; }

        [Required]
        // Nội dung chi tiết của thông báo.
        public string Message { get; set; }
    }
}
