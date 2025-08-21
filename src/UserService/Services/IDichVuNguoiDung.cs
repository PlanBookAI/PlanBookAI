using UserService.Models.DTOs;

namespace UserService.Services;

public interface IDichVuNguoiDung
{
    // Profile management
    Task<PhanHoiHoSo> LayHoSoAsync(Guid userId);
    Task<PhanHoiHoSo> CapNhatHoSoAsync(Guid userId, YeuCauCapNhatHoSo yeuCau);
    
    // Admin functions
    Task<PhanHoiDanhSachNguoiDung> LayDanhSachNguoiDungAsync(Guid adminId);
    Task<PhanHoiHoSo> LayThongTinNguoiDungAsync(Guid adminId, Guid userId);
    Task<PhanHoiHoSo> XoaNguoiDungAsync(Guid adminId, Guid userId);
    Task<PhanHoiHoSo> KhoiPhucNguoiDungAsync(Guid adminId, Guid userId);
    
    // User management
    Task<bool> KiemTraNguoiDungTonTaiAsync(Guid userId);
    Task<bool> KiemTraEmailTonTaiAsync(string email);

    // Bổ sung các phương thức cho tính năng Quên Mật khẩu
    Task<PhanHoiHoSo> QuenMatKhauAsync(string email);
    Task<PhanHoiHoSo> XacThucOtpAsync(string email, string otp);
    Task<PhanHoiHoSo> DatLaiMatKhauAsync(string email, string otp, string matKhauMoi);
}
