using AuthService.Models.Entities;

namespace AuthService.Services
{
    public interface IDichVuJWT
    {
        /// <summary>
        /// Tạo JWT access token cho user
        /// </summary>
        /// <param name="nguoiDung">Thông tin user</param>
        /// <param name="vaiTro">Thông tin vai trò</param>
        /// <returns>JWT token string</returns>
        string TaoAccessToken(NguoiDung nguoiDung, VaiTro vaiTro);

        /// <summary>
        /// Tạo refresh token
        /// </summary>
        /// <param name="nguoiDungId">ID của user</param>
        /// <param name="ghiNho">Có ghi nhớ đăng nhập không</param>
        /// <returns>Refresh token string</returns>
        string TaoRefreshToken(Guid nguoiDungId, bool ghiNho = false);

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID nếu valid, null nếu invalid</returns>
        Guid? ValidateToken(string token);

        /// <summary>
        /// Lấy user ID từ JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID</returns>
        Guid? LayUserIdTuToken(string token);

        /// <summary>
        /// Kiểm tra token có hết hạn không
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>True nếu hết hạn</returns>
        bool KiemTraTokenHetHan(string token);

        /// <summary>
        /// Lấy thời gian hết hạn của token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Thời gian hết hạn</returns>
        DateTime? LayThoiGianHetHan(string token);
    }
}
