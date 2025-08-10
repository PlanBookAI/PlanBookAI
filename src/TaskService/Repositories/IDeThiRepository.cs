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
        Task<List<DeThi>> GetAllAsync();

        // Lấy một đề thi theo ID
        Task<DeThi?> GetByIdAsync(string id);

        // Thêm một đề thi mới
        Task<DeThi> CreateAsync(DeThi deThi);

        // Cập nhật thông tin một đề thi
        Task<DeThi> UpdateAsync(DeThi deThi);

        // Xóa một đề thi theo ID
        Task DeleteAsync(string id);
    }
}
