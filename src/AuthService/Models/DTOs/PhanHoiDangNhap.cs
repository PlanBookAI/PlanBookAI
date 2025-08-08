namespace AuthService.Models.DTOs
{
    public class PhanHoiDangNhap
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? HetHanLuc { get; set; }
        public ThongTinNguoiDung? NguoiDung { get; set; }
    }

    public class ThongTinNguoiDung
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public bool LaHoatDong { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
    }
}
