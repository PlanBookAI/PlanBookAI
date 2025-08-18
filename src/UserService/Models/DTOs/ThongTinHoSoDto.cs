namespace UserService.Models.DTOs;

public class ThongTinHoSoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? MoTaBanThan { get; set; }
    public string? AnhDaiDienUrl { get; set; }
    public DateTime TaoLuc { get; set; }
    public DateTime CapNhatLuc { get; set; }
}
