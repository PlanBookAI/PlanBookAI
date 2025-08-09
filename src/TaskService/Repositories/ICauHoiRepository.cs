using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    // Giao diện (interface) cho repository của CauHoi
    public interface ICauHoiRepository
    {
        // Lấy tất cả các câu hỏi
        Task<IEnumerable<CauHoi>> GetAllAsync();

        // Lấy một câu hỏi theo ID
        Task<CauHoi> GetByIdAsync(Guid id);

        // Thêm một câu hỏi mới
        Task AddAsync(CauHoi cauHoi);

        // Cập nhật thông tin một câu hỏi
        Task UpdateAsync(CauHoi cauHoi);

        // Xóa một câu hỏi theo ID
        Task DeleteAsync(Guid id);
    }
}
