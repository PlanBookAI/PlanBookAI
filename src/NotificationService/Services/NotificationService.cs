
using NotificationService.Models;
using NotificationService.Models.DTOs;
using NotificationService.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    // Lớp này chứa logic nghiệp vụ liên quan đến thông báo.
    public class NotificationService
    {
        // Sử dụng Dependency Injection để tiêm NotificationRepository.
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Tạo một thông báo mới từ DTO.
        /// </summary>
        /// <param name="createDto">DTO chứa dữ liệu thông báo.</param>
        /// <returns>Đối tượng thông báo đã được tạo.</returns>
        public async Task<Notification> CreateNotification(CreateNotificationDto createDto)
        {
            // Chuyển đổi DTO sang Model.
            var notification = new Notification
            {
                UserId = createDto.UserId,
                Title = createDto.Title,
                Message = createDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            // Gọi Repository để lưu thông báo vào database.
            return await _notificationRepository.Create(notification);
        }

        /// <summary>
        /// Lấy tất cả thông báo của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Danh sách các thông báo.</returns>
        public async Task<IEnumerable<Notification>> GetNotificationsByUserId(string userId)
        {
            // Gọi Repository để lấy dữ liệu.
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
}
