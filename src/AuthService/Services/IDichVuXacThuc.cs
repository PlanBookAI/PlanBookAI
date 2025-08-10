using AuthService.Models.DTOs;
using System.Threading.Tasks;

namespace AuthService.Services
{
    /// <summary>
    /// Interface định nghĩa các phương thức chính của dịch vụ xác thực.
    /// </summary>
    public interface IDichVuXacThuc
    {
        Task<PhanHoiXacThucDto> TaoToken(string email, string matKhau); // Tạo token truy cập và refresh token.
        Task<PhanHoiXacThucDto> DangKy(string email, string matKhau); // Đăng ký tài khoản mới.
        Task<PhanHoiXacThucDto> LamMoiToken(string refreshToken);
        string XacThucToken(string accessToken); // Xác thực token truy cập và trả về tên người dùng.
    }
}
