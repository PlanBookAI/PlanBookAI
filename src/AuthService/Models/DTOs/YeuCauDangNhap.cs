using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs;

public class YeuCauDangNhap
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string MatKhau { get; set; } = string.Empty;
}
