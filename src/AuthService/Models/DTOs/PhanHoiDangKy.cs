namespace AuthService.Models.DTOs;

public class PhanHoiDangKy
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public Guid? NguoiDungId { get; set; }
}
