namespace AuthService.Models.DTOs;

public class PhanHoiXacThucDto
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? HetHanLuc { get; set; }
}
