using UserService.Models.DTOs;

namespace UserService.Models.DTOs;

public class PhanHoiDanhSachNguoiDung
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public List<ThongTinNguoiDungDto>? DanhSach { get; set; }
    public int TongSo { get; set; }
}
