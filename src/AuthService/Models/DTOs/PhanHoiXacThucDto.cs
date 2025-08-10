using System;

namespace AuthService.Models.DTOs
{
    /// <summary>
    /// DTO cho phản hồi xác thực
    /// </summary>
    public class PhanHoiXacThucDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
