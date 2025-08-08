using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs
{
    public class YeuCauDangNhap
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8-100 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        public bool GhiNho { get; set; } = false;
    }
}
