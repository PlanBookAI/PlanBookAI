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
}
