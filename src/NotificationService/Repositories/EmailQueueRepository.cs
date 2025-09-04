using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Repositories
{
    // Lớp này đại diện cho tầng Repository để tương tác với bảng email_queue.
    public class EmailQueueRepository
    {
        // Giả lập một database context.
        private readonly List<EmailQueue> _context;

        public EmailQueueRepository()
        {
            _context = new List<EmailQueue>();
        }

        /// <summary>
        /// Thêm một email mới vào hàng đợi.
        /// </summary>
        /// <param name="email">Đối tượng email cần thêm.</param>
        /// <returns>Đối tượng email đã được lưu.</returns>
        public async Task<EmailQueue> Create(EmailQueue email)
        {
            email.Id = Guid.NewGuid();
            email.Status = "PENDING";
            email.Retries = 0;
            email.CreatedAt = DateTime.UtcNow;
            _context.Add(email);
            return await Task.FromResult(email);
        }

        /// <summary>
        /// Lấy danh sách các email có trạng thái PENDING để gửi.
        /// </summary>
        /// <returns>Danh sách các email đang chờ xử lý.</returns>
        public async Task<IEnumerable<EmailQueue>> GetPendingEmails()
        {
            return await Task.FromResult(_context.Where(e => e.Status == "PENDING").ToList());
        }

        /// <summary>
        /// Cập nhật trạng thái và số lần thử lại của một email.
        /// </summary>
        /// <param name="id">ID của email.</param>
        /// <param name="status">Trạng thái mới (ví dụ: "SENT", "FAILED").</param>
        /// <param name="errorLog">Thông tin lỗi (nếu có).</param>
        /// <returns>True nếu cập nhật thành công, False nếu không tìm thấy.</returns>
        public async Task<bool> UpdateStatus(Guid id, string status, string errorLog = null)
        {
            var email = _context.FirstOrDefault(e => e.Id == id);
            if (email == null) return await Task.FromResult(false);

            email.Status = status;
            email.SentAt = (status == "SENT") ? DateTime.UtcNow : email.SentAt;
            email.ErrorLog = errorLog;
            if (status == "FAILED")
            {
                email.Retries++;
            }
            return await Task.FromResult(true);
        }
    }
}
