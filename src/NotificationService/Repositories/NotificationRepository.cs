using NotificationService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Repositories
{
    public class NotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tạo một thông báo mới trong cơ sở dữ liệu.
        public async Task<Notification> Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        // Tạo nhiều thông báo mới cùng lúc.
        public async Task CreateMany(IEnumerable<Notification> notifications)
        {
            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        // Lấy thông báo theo ID.
        public async Task<Notification> FindById(Guid id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        // Lấy danh sách thông báo theo ID người dùng.
        public async Task<IEnumerable<Notification>> FindByUserId(string userId)
        {
            return await _context.Notifications
                                 .Where(n => n.UserId == userId)
                                 .OrderByDescending(n => n.CreatedAt)
                                 .ToListAsync();
        }

        // Cập nhật trạng thái của thông báo.
        public async Task<bool> Update(Notification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(notification.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        // Đánh dấu một thông báo là đã đọc.
        public async Task<bool> UpdateIsRead(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return false;
            }
            notification.IsRead = true;
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa một thông báo.
        public async Task<bool> Delete(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return false;
            }
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy danh sách các thông báo đã lên lịch cần được gửi.
        public async Task<IEnumerable<Notification>> GetScheduledNotifications()
        {
            return await _context.Notifications
                                 .Where(n => n.Status == "SCHEDULED" && n.ScheduledAt <= DateTime.UtcNow)
                                 .ToListAsync();
        }

        private bool NotificationExists(Guid id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }

    // Lớp DbContext giả lập. Trong thực tế, bạn sẽ tạo một lớp DbContext thật.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}
