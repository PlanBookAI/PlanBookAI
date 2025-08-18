using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs;

public class YeuCauDangKy
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [Range(1, 4, ErrorMessage = "Vai trò không hợp lệ")]
    public int VaiTroId { get; set; }
}
