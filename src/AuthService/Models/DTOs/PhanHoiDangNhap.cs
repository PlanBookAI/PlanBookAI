namespace AuthService.Models.DTOs;

public class PhanHoiDangNhap
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? HetHanLuc { get; set; }
    public ThongTinNguoiDung? ThongTinNguoiDung { get; set; }
}

public class ThongTinNguoiDung
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string TenVaiTro { get; set; } = string.Empty;
    public int VaiTroId { get; set; }
}
