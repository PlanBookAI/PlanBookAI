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
            _logger.LogInformation("D?ch v? Notification Worker ?� kh?i ??ng.");

            // H?n gi? ?? worker ch?y m?i 60 gi�y.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Worker ?ang ki?m tra c�c th�ng b�o ?� l�n l?ch.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationRepository = scope.ServiceProvider.GetRequiredService<NotificationRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                // L?y c�c th�ng b�o ?� ??n h?n t? repository.
                var dueNotifications = await notificationRepository.GetScheduledNotifications();

                if (dueNotifications.Any())
                {
                    _logger.LogInformation($"T�m th?y {dueNotifications.Count()} th�ng b�o c?n x? l�.");
                    foreach (var notification in dueNotifications)
                    {
                        try
                        {
                            // Logic g?i th�ng b�o th?c t?
                            _logger.LogInformation($"?ang x? l� th�ng b�o {notification.Id}...");

                            // G?i th�ng b�o b?ng service
                            await notificationService.SendNotification(notification);

                            // C?p nh?t tr?ng th�i sau khi g?i th�nh c�ng
                            await notificationRepository.UpdateProcessingStatus(notification.Id, true, notification.RetryCount);
                            _logger.LogInformation($"Th�ng b�o {notification.Id} ?� ???c g?i th�nh c�ng.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"C� l?i khi x? l� th�ng b�o {notification.Id}.");
                            // Logic x? l� l?i v� retry
                            await notificationRepository.UpdateProcessingStatus(notification.Id, false, notification.RetryCount + 1);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Kh�ng t�m th?y th�ng b�o n�o ?� l�n l?ch.");
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
