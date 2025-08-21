using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
    // DTO cho yêu cầu quên mật khẩu
    public class YeuCauQuenMatKhau
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;
    }

    // DTO cho yêu cầu xác thực OTP
    public class YeuCauXacThucOtpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc.")]
        public string Otp { get; set; } = string.Empty;
    }

    // DTO cho yêu cầu đặt lại mật khẩu
    public class YeuCauDatLaiMatKhauDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc.")]
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}
