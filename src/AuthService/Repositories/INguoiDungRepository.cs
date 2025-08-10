using AuthService.Models.Entities;
using System;
using System.Threading.Tasks;

namespace AuthService.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức để thao tác với dữ liệu người dùng.
    /// </summary>
    public interface INguoiDungRepository
    {
        Task<NguoiDung> GetByTenDangNhapAsync(string tenDangNhap); // Tìm người dùng theo tên đăng nhập.
        Task<NguoiDung> GetByRefreshTokenAsync(string refreshToken); // Tìm người dùng theo refresh token.
        Task AddAsync(NguoiDung nguoiDung); 
        Task UpdateAsync(NguoiDung nguoiDung);
    }
}
