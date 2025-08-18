using UserService.Models.DTOs;

namespace UserService.Services;

public interface IDichVuLichSuDangNhap
{
    Task<PhanHoiLichSuDangNhap> LogDangNhapAsync(Guid userId, string token);
    Task<PhanHoiLichSuDangNhap> LayLichSuDangNhapAsync(Guid logId);
    Task<PhanHoiDanhSachLichSuDangNhap> LayDanhSachLichSuDangNhapAsync(Guid userId);
    Task<PhanHoiLichSuDangNhap> XoaLichSuDangNhapAsync(Guid logId);
    Task<PhanHoiLichSuDangNhap> XoaLichSuDangNhapCuaUserAsync(Guid userId);
}
