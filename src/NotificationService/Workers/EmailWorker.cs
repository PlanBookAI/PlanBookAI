
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using NotificationService.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Workers
{
    // Kế thừa từ BackgroundService để chạy như một dịch vụ nền.
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        // Số lần thử lại tối đa.
        private const int MaxRetries = 5;

        public EmailWorker(ILogger<EmailWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        // Phương thức chính của worker, sẽ chạy liên tục.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Worker đang chạy.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessEmailQueueAsync();

                // Chờ 1 phút trước khi kiểm tra hàng đợi lại.
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Email Worker đã dừng.");
        }

        private async Task ProcessEmailQueueAsync()
        {
            try
            {
                // Tạo một scope để lấy các services cần thiết.
                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailQueueRepo = scope.ServiceProvider.GetRequiredService<EmailQueueRepository>();

                    // Lấy tất cả các email có trạng thái 'PENDING'.
                    var emailsToProcess = (await emailQueueRepo.GetPendingEmails()).ToList();

                    foreach (var email in emailsToProcess)
                    {
                        try
                        {
                            await SendEmailAsync(email);
                            // Cập nhật trạng thái sau khi gửi thành công.
                            await emailQueueRepo.UpdateStatus(email.Id, "SENT");
                            _logger.LogInformation($"Đã gửi email thành công cho: {email.Recipient}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Lỗi khi gửi email cho: {email.Recipient}");

                            // Tăng số lần thử lại
                            email.Retries++;

                            if (email.Retries < MaxRetries)
                            {
                                // Áp dụng exponential backoff cho thời gian chờ
                                // Ví dụ: 2^retries giây.
                                var delayInSeconds = Math.Pow(2, email.Retries);
                                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));

                                // Cập nhật trạng thái và số lần thử lại.
                                await emailQueueRepo.UpdateStatus(email.Id, "PENDING", ex.Message);
                            }
                            else
                            {
                                // Nếu đã đạt số lần thử tối đa, đánh dấu là 'FAILED'.
                                await emailQueueRepo.UpdateStatus(email.Id, "FAILED", $"Thử lại thất bại sau {MaxRetries} lần. {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình xử lý hàng đợi email.");
            }
        }

        // Phương thức gửi email thực tế.
        private async Task SendEmailAsync(Models.EmailQueue email)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Sender Name", "your_email@gmail.com")); // Thay đổi thông tin người gửi
            mimeMessage.To.Add(new MailboxAddress("", email.Recipient));
            mimeMessage.Subject = email.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = email.Body; // Giả sử nội dung là HTML

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Cấu hình SMTP client.
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("your_email@gmail.com", "your_app_password"); // Thay đổi thông tin xác thực
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
