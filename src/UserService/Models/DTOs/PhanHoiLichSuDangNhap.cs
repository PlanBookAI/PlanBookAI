using UserService.Models.DTOs;

namespace UserService.Models.DTOs;

public class PhanHoiLichSuDangNhap
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public ThongTinLichSuDangNhapDto? DuLieu { get; set; }
}

public class PhanHoiDanhSachLichSuDangNhap
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; } = string.Empty;
    public List<ThongTinLichSuDangNhapDto>? DuLieu { get; set; }
    public int TongSo { get; set; }
}
