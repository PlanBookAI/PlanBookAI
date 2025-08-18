using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs;

public class LamMoiTokenDto
{
    [Required(ErrorMessage = "Refresh token là bắt buộc")]
    public string RefreshToken { get; set; } = string.Empty;
}
