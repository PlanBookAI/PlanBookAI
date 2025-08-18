using AuthService.Models.DTOs;

namespace AuthService.Services;

public interface IDichVuXacThuc
{
    Task<PhanHoiDangNhap> DangNhapAsync(YeuCauDangNhap yeuCau);
    Task<PhanHoiDangKy> DangKyAsync(YeuCauDangKy yeuCau);
    Task<PhanHoiXacThucDto> LamMoiTokenAsync(string refreshToken);
    Task<bool> DangXuatAsync(string token);
}
