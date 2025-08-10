using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
    public class YeuCauCapNhatHoSo
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ tên phải có độ dài từ 2 đến 255 ký tự.")]
        public string HoTen { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string? SoDienThoai { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
        public string? DiaChi { get; set; }

        [Url(ErrorMessage = "URL ảnh đại diện không hợp lệ.")]
        [StringLength(500, ErrorMessage = "URL không được vượt quá 500 ký tự.")]
        public string? AnhDaiDienUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? MoTaBanThan { get; set; }
    }
}