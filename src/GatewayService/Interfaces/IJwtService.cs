namespace GatewayService.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Xác thực JWT token và trả về user ID
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID nếu token hợp lệ, null nếu không hợp lệ</returns>
        Task<string?> ValidateTokenAsync(string token);

        /// <summary>
        /// Kiểm tra token có hết hạn không
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>True nếu token hết hạn</returns>
        bool IsTokenExpired(string token);

        /// <summary>
        /// Lấy thông tin user từ token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User claims</returns>
        Task<Dictionary<string, string>?> GetUserClaimsAsync(string token);
    }
}
