
using NotificationService.Models;
using NotificationService.Models.DTOs;
using NotificationService.Repositories;
using System;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    // Lớp này chứa logic nghiệp vụ liên quan đến việc gửi email.
    public class EmailService
    {
        private readonly EmailQueueRepository _emailQueueRepository;
        // Thực tế, bạn sẽ tiêm thêm một service khác để gửi email qua SMTP tại đây,
        // nhưng hiện tại chúng ta sẽ chỉ tập trung vào việc tạo bản ghi trong hàng đợi.
        // private readonly ISmtpClient _smtpClient;

        public EmailService(EmailQueueRepository emailQueueRepository)
        {
            _emailQueueRepository = emailQueueRepository;
        }

        /// <summary>
        /// Xử lý yêu cầu gửi email trực tiếp từ các service khác
        /// và đưa email vào hàng đợi để xử lý bất đồng bộ.
        /// </summary>
        /// <param name="sendEmailDto">DTO chứa thông tin email.</param>
        public async Task<EmailQueue> SendEmailAsync(SendEmailDto sendEmailDto)
        {
            // Chuyển đổi DTO sang EmailQueue Model.
            var emailRecord = new EmailQueue
            {
                Recipient = sendEmailDto.Recipient,
                Subject = sendEmailDto.Subject,
                Body = sendEmailDto.Body,
                Status = "PENDING", // Trạng thái ban đầu là PENDING
                Retries = 0,
                CreatedAt = DateTime.UtcNow
            };

            // Gọi Repository để lưu bản ghi vào hàng đợi.
            return await _emailQueueRepository.Create(emailRecord);
        }

        // Tạm thời bỏ qua các phương thức hỗ trợ UserService
        // (gửi email OTP, xác nhận, bảo mật)
        // Chúng ta sẽ quay lại các phương thức này khi tích hợp hoàn chỉnh hơn.
    }
}
