
using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTOs
{
    // Lớp này dùng để truyền dữ liệu khi gửi một yêu cầu gửi email.
    public class SendEmailDto
    {
        [Required]
        [EmailAddress]
        // Địa chỉ email của người nhận.
        public string Recipient { get; set; }

        [Required]
        // Tiêu đề của email.
        public string Subject { get; set; }

        [Required]
        // Nội dung của email.
        public string Body { get; set; }
    }
}
