using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu làm mới token
    /// </summary>
    public class LamMoiTokenDto
    {
        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
