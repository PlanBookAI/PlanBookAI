using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Repositories;
using NotificationService.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Workers
{
    public class NotificationWorker : IHostedService, IDisposable
    {
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public NotificationWorker(ILogger<NotificationWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Dịch vụ Notification Worker đã khởi động.");

            // Hẹn giờ để worker chạy mỗi 60 giây.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Worker đang kiểm tra các thông báo chưa đọc.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationRepository = scope.ServiceProvider.GetRequiredService<NotificationRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService.Services.NotificationService>();

                // Lấy các thông báo chưa đọc
                var unreadNotifications = await notificationRepository.GetUnreadNotifications(Guid.Empty);

                if (unreadNotifications.Any())
                {
                    _logger.LogInformation($"Tìm thấy {unreadNotifications.Count()} thông báo chưa đọc.");
                    foreach (var notification in unreadNotifications)
                    {
                        try
                        {
                            _logger.LogInformation($"Đang xử lý thông báo {notification.Id}...");
                            await notificationService.SendNotification(notification);
                            _logger.LogInformation($"Thông báo {notification.Id} đã được gửi thành công.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Có lỗi khi xử lý thông báo {notification.Id}.");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Không tìm thấy thông báo nào chưa đọc.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Dịch vụ Notification Worker đang dừng lại.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
