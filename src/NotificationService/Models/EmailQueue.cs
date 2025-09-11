using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Models
{
    // Lớp này định nghĩa cấu trúc của một email trong hàng đợi.
    [Table("email_queue", Schema = "notifications")]
    public class EmailQueue
    {
        // Khóa chính của bảng.
        [Key]
        public Guid Id { get; set; }

        // Người nhận email.
        public string Recipient { get; set; }

        // Tiêu đề của email.
        public string Subject { get; set; }

        // Nội dung của email, có thể là văn bản thuần túy hoặc HTML.
        public string Body { get; set; }

        // Trạng thái hiện tại của email (ví dụ: PENDING, SENT, FAILED).
        public string Status { get; set; }

        // Số lần thử lại gửi email.
        public int Retries { get; set; }

        // Thời gian email được tạo và đưa vào hàng đợi.
        public DateTime CreatedAt { get; set; }

        // Thời gian email được gửi thành công.
        public DateTime? SentAt { get; set; }

        // Ghi chú lỗi (nếu có).
        public string ErrorLog { get; set; }
    }
}
