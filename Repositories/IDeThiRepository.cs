using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    // Giao diện (interface) cho repository của DeThi
    public interface IDeThiRepository
    {
        // Lấy tất cả các đề thi
        Task<IEnumerable<DeThi>> GetAllAsync();

        // Lấy một đề thi theo ID
        Task<DeThi> GetByIdAsync(Guid id);

        // Thêm một đề thi mới
        Task AddAsync(DeThi deThi);

        // Cập nhật thông tin một đề thi
        Task UpdateAsync(DeThi deThi);

        // Xóa một đề thi theo ID
        Task DeleteAsync(Guid id);
    }
}
