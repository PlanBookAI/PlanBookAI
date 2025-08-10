using AuthService.Models.DTOs;
using System.Threading.Tasks;

namespace AuthService.Services.DichVuXacThuc
{
    /// <summary>
    /// Interface định nghĩa các phương thức chính của dịch vụ xác thực.
    /// </summary>
    public interface IDichVuXacThuc
    {
        Task<PhanHoiXacThucDto> TaoToken(string tenDangNhap, string matKhau); // Tạo token truy cập và refresh token.
        Task<PhanHoiXacThucDto> LamMoiToken(string refreshToken); 
        string XacThucToken(string accessToken); // Xác thực token truy cập và trả về tên người dùng.
    }
}