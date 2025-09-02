
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Models
{
    // Lớp này định nghĩa cấu trúc của một thông báo.
    [Table("notifications", Schema = "notifications")]
    public class Notification
    {
        // Khóa chính của bảng.
        [Key]
        public Guid Id { get; set; }

        // ID của người dùng nhận thông báo.
        public string UserId { get; set; }

        // Tiêu đề của thông báo.
        public string Title { get; set; }

        // Nội dung chi tiết của thông báo.
        public string Message { get; set; }

        // Trạng thái đã đọc của thông báo (mặc định là false).
        public bool IsRead { get; set; } = false;

        // Thời gian tạo thông báo.
        public DateTime CreatedAt { get; set; }
    }
}
