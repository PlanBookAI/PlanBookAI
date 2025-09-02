using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Repositories
{
    // Lớp này đại diện cho tầng Repository để tương tác với bảng notifications trong cơ sở dữ liệu.
    public class NotificationRepository
    {
        // Giả lập một database context. Trong thực tế, bạn sẽ thay thế nó bằng
        // một đối tượng DbContext của Entity Framework Core.
        private readonly List<Notification> _context;

        public NotificationRepository()
        {
            _context = new List<Notification>();
        }

        /// <summary>
        /// Tạo một thông báo mới trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="notification">Đối tượng thông báo cần tạo.</param>
        /// <returns>Đối tượng thông báo đã được lưu.</returns>
        public async Task<Notification> Create(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            _context.Add(notification);
            return await Task.FromResult(notification);
        }

        /// <summary>
        /// Lấy tất cả thông báo của một người dùng cụ thể.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Danh sách các thông báo của người dùng đó.</returns>
        public async Task<IEnumerable<Notification>> FindByUserId(string userId)
        {
            return await Task.FromResult(_context.Where(n => n.UserId == userId).ToList());
        }

        /// <summary>
        /// Lấy chi tiết một thông báo dựa trên ID.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>Đối tượng thông báo hoặc null nếu không tìm thấy.</returns>
        public async Task<Notification> FindById(Guid id)
        {
            return await Task.FromResult(_context.FirstOrDefault(n => n.Id == id));
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// </summary>
        /// <param name="id">ID của thông báo.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy.</returns>
        public async Task<bool> UpdateIsRead(Guid id)
        {
            var notification = _context.FirstOrDefault(n => n.Id == id);
            if (notification == null) return await Task.FromResult(false);

            notification.IsRead = true;
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Xóa một thông báo khỏi cơ sở dữ liệu.
        /// </summary>
        /// <param name="id">ID của thông báo cần xóa.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy.</returns>
        public async Task<bool> Delete(Guid id)
        {
            var notification = _context.FirstOrDefault(n => n.Id == id);
            if (notification == null) return await Task.FromResult(false);

            _context.Remove(notification);
            return await Task.FromResult(true);
        }
    }
}

