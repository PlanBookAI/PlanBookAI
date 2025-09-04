using NotificationService.Models;
using NotificationService.Models.DTOs;
using NotificationService.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NotificationService.Services
{
    // Lớp này chứa logic nghiệp vụ liên quan đến thông báo.
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly TemplateEngine _templateEngine;

        public NotificationService(NotificationRepository notificationRepository, TemplateEngine templateEngine)
        {
            _notificationRepository = notificationRepository;
            _templateEngine = templateEngine;
        }

        /// <summary>
        /// Tạo một thông báo mới từ DTO.
        /// Phương thức này được dùng cho các thông báo đơn lẻ, gửi ngay lập tức.
        /// </summary>
        /// <param name="createDto">DTO chứa dữ liệu thông báo.</param>
        /// <returns>Đối tượng thông báo đã được tạo.</returns>
        public async Task<Notification> CreateNotification(CreateNotificationDto createDto)
        {
            var notification = new Notification
            {
                UserId = createDto.UserId,
                Title = createDto.Title,
                Message = createDto.Message,
                Type = "INFO",
                IsRead = false
            };
            return await _notificationRepository.Create(notification);
        }

        /// <summary>
        /// Gửi thông báo hàng loạt cho nhiều người dùng.
        /// </summary>
        /// <param name="bulkDto">DTO chứa thông tin gửi hàng loạt.</param>
        public async Task SendBulkNotification(BulkNotificationRequestDto bulkDto)
        {
            // Giả lập logic để lấy template từ database.
            // Trong một hệ thống thực tế, bạn sẽ dùng TemplateId để lấy template.
            var template = new NotificationTemplate
            {
                TitleTemplate = "Thông báo quan trọng từ {{AppName}}",
                ContentTemplate = "Xin chào {{UserName}}, bạn có một thông báo mới: {{Content}}"
            };

            var notifications = new List<Notification>();
            foreach (var userId in bulkDto.UserIds)
            {
                var variables = new Dictionary<string, string>
                {
                    { "AppName", "Hệ thống thông báo" },
                    { "UserName", "Người dùng " + userId },
                    { "Content", "Nội dung thông báo hàng loạt" }
                };

                // Render nội dung thông báo cho từng người dùng.
                var renderedTitle = _templateEngine.Render(template.TitleTemplate, variables);
                var renderedContent = _templateEngine.Render(template.ContentTemplate, variables);

                notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = renderedTitle,
                    Message = renderedContent,
                    Type = "INFO",
                    IsRead = false
                });
            }
            // Lưu tất cả các thông báo vào database.
            await _notificationRepository.CreateMany(notifications);
        }

        /// <summary>
        /// Lên lịch gửi một thông báo.
        /// </summary>
        /// <param name="scheduledDto">DTO chứa thông tin lên lịch.</param>
        public async Task ScheduleNotification(ScheduledNotificationRequestDto scheduledDto)
        {
            // Giả lập logic để lấy template từ database.
            var template = new NotificationTemplate
            {
                TitleTemplate = "Nhắc nhở từ hệ thống: {{TaskName}}",
                ContentTemplate = "Công việc {{TaskName}} của bạn sẽ đến hạn vào {{DueTime}}. Hãy kiểm tra!"
            };

            // Render nội dung thông báo.
            var renderedTitle = _templateEngine.Render(template.TitleTemplate, scheduledDto.Variables);
            var renderedContent = _templateEngine.Render(template.ContentTemplate, scheduledDto.Variables);

            var notification = new Notification
            {
                UserId = scheduledDto.UserId,
                Title = renderedTitle,
                Message = renderedContent,
                Type = "INFO",
                IsRead = false
            };
            await _notificationRepository.Create(notification);
        }

        /// <summary>
        /// Hủy một thông báo đã lên lịch.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy hoặc không thể hủy.</returns>
        public async Task<bool> CancelScheduledNotification(Guid id)
        {
            var notification = await _notificationRepository.FindById(id);
            if (notification == null)
            {
                return false;
            }
            await _notificationRepository.Update(notification);
            return true;
        }

        /// <summary>
        /// Gửi thông báo thực tế (giả lập).
        /// </summary>
        /// <param name="notification">Đối tượng thông báo cần gửi.</param>
        public async Task SendNotification(Notification notification)
        {
            // Giả lập logic gửi thông báo thực tế (ví dụ: Push Notification, Email).
            // Bạn cần thêm logic tích hợp với các dịch vụ gửi thông báo ở đây.
            Console.WriteLine($"Đang gửi thông báo {notification.Id} tới người dùng {notification.UserId}...");
            await Task.Delay(100);
            Console.WriteLine($"Gửi thành công thông báo: '{notification.Title}'");
        }

        /// <summary>
        /// Lấy tất cả thông báo của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Danh sách các thông báo.</returns>
        public async Task<IEnumerable<Notification>> GetNotificationsByUserId(Guid userId)
        {
            return await _notificationRepository.FindByUserId(userId);
        }

        /// <summary>
        /// Lấy chi tiết một thông báo.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>Đối tượng thông báo hoặc null.</returns>
        public async Task<Notification> GetNotificationById(Guid id)
        {
            return await _notificationRepository.FindById(id);
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy.</returns>
        public async Task<bool> MarkAsRead(Guid id)
        {
            return await _notificationRepository.UpdateIsRead(id);
        }

        /// <summary>
        /// Xóa một thông báo.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy.</returns>
        public async Task<bool> DeleteNotification(Guid id)
        {
            return await _notificationRepository.Delete(id);
        }
    }

    // Các lớp giả lập được đặt ở đây để file này có thể chạy được.
    // Trong thực tế, bạn nên tách chúng ra các file riêng biệt.

    public class TemplateEngine
    {
        public string Render(string template, object variables)
        {
            var result = template;
            if (variables is Dictionary<string, string> dict)
            {
                foreach (var kvp in dict)
                {
                    result = result.Replace("{{" + kvp.Key + "}}", kvp.Value);
                }
            }
            return result;
        }
    }

    public class NotificationTemplate
    {
        public string TitleTemplate { get; set; }
        public string ContentTemplate { get; set; }
    }

}
