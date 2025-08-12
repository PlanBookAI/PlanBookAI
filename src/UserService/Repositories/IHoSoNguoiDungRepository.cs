using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Repositories
{
    // Giao diện (interface) cho repository của HoSoNguoiDung
    public interface IHoSoNguoiDungRepository
    {
        // Lấy tất cả hồ sơ người dùng
        Task<IEnumerable<HoSoNguoiDung>> GetAllAsync();

        // Lấy một hồ sơ người dùng theo ID
        Task<HoSoNguoiDung?> GetByIdAsync(string id);

        // Thêm một hồ sơ người dùng mới
        Task AddAsync(HoSoNguoiDung hoSo);

        // Cập nhật thông tin một hồ sơ người dùng
        Task UpdateAsync(HoSoNguoiDung hoSo);

        // Xóa một hồ sơ người dùng theo ID
        Task DeleteAsync(string id);
    }
}