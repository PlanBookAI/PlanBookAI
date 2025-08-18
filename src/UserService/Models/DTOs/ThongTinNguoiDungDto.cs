namespace UserService.Models.DTOs;

public class ThongTinNguoiDungDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
    public bool HoatDong { get; set; }
    public bool DaXoa { get; set; }
    public DateTime? NgayXoa { get; set; }
    public DateTime TaoLuc { get; set; }
    public DateTime CapNhatLuc { get; set; }
    
    // Profile fields
    public string HoTen { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? AnhDaiDienUrl { get; set; }
}
