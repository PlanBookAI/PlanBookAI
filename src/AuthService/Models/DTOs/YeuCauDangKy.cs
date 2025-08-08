using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs
{
    public class YeuCauDangKy
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8-100 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2-255 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Range(1, 4, ErrorMessage = "Vai trò không hợp lệ")]
        public int VaiTroId { get; set; }
    }
}
