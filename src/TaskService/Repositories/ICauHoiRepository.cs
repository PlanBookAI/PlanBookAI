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
        Task<List<CauHoi>> GetAllAsync();

        // Lấy một câu hỏi theo ID
        Task<CauHoi?> GetByIdAsync(string id);

        // Lấy câu hỏi theo môn học và độ khó
        Task<List<CauHoi>> GetByMonHocAndDoKhoAsync(string monHoc, string doKho);

        // Thêm một câu hỏi mới
        Task<CauHoi> CreateAsync(CauHoi cauHoi);

        // Cập nhật thông tin một câu hỏi
        Task<CauHoi> UpdateAsync(CauHoi cauHoi);

        // Xóa một câu hỏi theo ID
        Task DeleteAsync(string id);
    }
}
