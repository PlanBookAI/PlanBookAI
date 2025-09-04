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
            _logger.LogInformation("D?ch v? Notification Worker ?ã kh?i ??ng.");

            // H?n gi? ?? worker ch?y m?i 60 giây.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Worker ?ang ki?m tra các thông báo ?ã lên l?ch.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationRepository = scope.ServiceProvider.GetRequiredService<NotificationRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                // L?y các thông báo ?ã ??n h?n t? repository.
                var dueNotifications = await notificationRepository.GetScheduledNotifications();

                if (dueNotifications.Any())
                {
                    _logger.LogInformation($"Tìm th?y {dueNotifications.Count()} thông báo c?n x? lý.");
                    foreach (var notification in dueNotifications)
                    {
                        try
                        {
                            // Logic g?i thông báo th?c t?
                            _logger.LogInformation($"?ang x? lý thông báo {notification.Id}...");

                            // G?i thông báo b?ng service
                            await notificationService.SendNotification(notification);

                            // C?p nh?t tr?ng thái sau khi g?i thành công
                            await notificationRepository.UpdateProcessingStatus(notification.Id, true, notification.RetryCount);
                            _logger.LogInformation($"Thông báo {notification.Id} ?ã ???c g?i thành công.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Có l?i khi x? lý thông báo {notification.Id}.");
                            // Logic x? lý l?i và retry
                            await notificationRepository.UpdateProcessingStatus(notification.Id, false, notification.RetryCount + 1);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Không tìm th?y thông báo nào ?ã lên l?ch.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("D?ch v? Notification Worker ?ang d?ng l?i.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
