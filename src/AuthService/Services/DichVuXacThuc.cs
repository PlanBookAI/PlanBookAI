using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using AuthService.Models.DTOs;
using AuthService.Models.Entities;
using AuthService.Repositories;
using AuthService.Data;

namespace AuthService.Services;

public class DichVuXacThuc : IDichVuXacThuc
{
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IVaiTroRepository _vaiTroRepository;
    private readonly IDichVuJWT _dichVuJWT;
    private readonly AuthDbContext _context;

    public DichVuXacThuc(
        INguoiDungRepository nguoiDungRepository,
        IVaiTroRepository vaiTroRepository,
        IDichVuJWT dichVuJWT,
        AuthDbContext context)
    {
        _nguoiDungRepository = nguoiDungRepository;
        _vaiTroRepository = vaiTroRepository;
        _dichVuJWT = dichVuJWT;
        _context = context;
    }

    public async Task<PhanHoiDangNhap> DangNhapAsync(YeuCauDangNhap yeuCau)
    {
        try
        {
            var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(yeuCau.Email);
            if (nguoiDung == null)
            {
                return new PhanHoiDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Email hoặc mật khẩu không đúng"
                };
            }

            if (!nguoiDung.HoatDong)
            {
                return new PhanHoiDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Tài khoản đã bị khóa"
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(yeuCau.MatKhau, nguoiDung.MatKhauMaHoa))
            {
                return new PhanHoiDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Email hoặc mật khẩu không đúng"
                };
            }

            // Cập nhật lần đăng nhập cuối
            await _nguoiDungRepository.UpdateLastLoginAsync(nguoiDung.Id);

            // Tạo token và refresh token
            var token = _dichVuJWT.TaoToken(nguoiDung);
            var refreshToken = _dichVuJWT.TaoRefreshToken();

            // Lưu session
            var phienDangNhap = new PhienDangNhap
            {
                NguoiDungId = nguoiDung.Id,
                Token = refreshToken,
                HetHanLuc = DateTime.UtcNow.AddDays(7)
            };

            _context.PhienDangNhaps.Add(phienDangNhap);
            await _context.SaveChangesAsync();

            return new PhanHoiDangNhap
            {
                ThanhCong = true,
                ThongBao = "Đăng nhập thành công",
                Token = token,
                RefreshToken = refreshToken,
                HetHanLuc = DateTime.UtcNow.AddMinutes(60),
                ThongTinNguoiDung = new ThongTinNguoiDung
                {
                    Id = nguoiDung.Id,
                    Email = nguoiDung.Email,
                    HoTen = nguoiDung.Email, // Sẽ cập nhật sau khi có user profile
                    TenVaiTro = nguoiDung.VaiTro.Ten,
                    VaiTroId = nguoiDung.VaiTroId
                }
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiDangKy> DangKyAsync(YeuCauDangKy yeuCau)
    {
        try
        {
            // Kiểm tra email đã tồn tại
            if (await _nguoiDungRepository.EmailExistsAsync(yeuCau.Email))
            {
                return new PhanHoiDangKy
                {
                    ThanhCong = false,
                    ThongBao = "Email đã được sử dụng"
                };
            }

            // Kiểm tra vai trò hợp lệ
            var vaiTro = await _vaiTroRepository.GetByIdAsync(yeuCau.VaiTroId);
            if (vaiTro == null)
            {
                return new PhanHoiDangKy
                {
                    ThanhCong = false,
                    ThongBao = "Vai trò không hợp lệ"
                };
            }

            // Mã hóa mật khẩu
            var matKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(yeuCau.MatKhau);

            // Tạo người dùng mới
            var nguoiDung = new NguoiDung
            {
                Email = yeuCau.Email,
                MatKhauMaHoa = matKhauMaHoa,
                VaiTroId = yeuCau.VaiTroId,
                HoatDong = true
            };

            var nguoiDungMoi = await _nguoiDungRepository.CreateAsync(nguoiDung);

            return new PhanHoiDangKy
            {
                ThanhCong = true,
                ThongBao = "Đăng ký thành công",
                NguoiDungId = nguoiDungMoi.Id
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiDangKy
            {
                ThanhCong = false,
                ThongBao = $"Lỗi đăng ký: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiXacThucDto> LamMoiTokenAsync(string refreshToken)
    {
        try
        {
            var phienDangNhap = await _context.PhienDangNhaps
                .Include(p => p.NguoiDung)
                .ThenInclude(n => n.VaiTro)
                .FirstOrDefaultAsync(p => p.Token == refreshToken && p.HetHanLuc > DateTime.UtcNow);

            if (phienDangNhap == null)
            {
                return new PhanHoiXacThucDto
                {
                    ThanhCong = false,
                    ThongBao = "Refresh token không hợp lệ hoặc đã hết hạn"
                };
            }

            var tokenMoi = _dichVuJWT.TaoToken(phienDangNhap.NguoiDung);

            return new PhanHoiXacThucDto
            {
                ThanhCong = true,
                ThongBao = "Làm mới token thành công",
                Token = tokenMoi,
                HetHanLuc = DateTime.UtcNow.AddMinutes(60)
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiXacThucDto
            {
                ThanhCong = false,
                ThongBao = $"Lỗi làm mới token: {ex.Message}"
            };
        }
    }

    public async Task<bool> DangXuatAsync(string token)
    {
        try
        {
            var phienDangNhap = await _context.PhienDangNhaps
                .FirstOrDefaultAsync(p => p.Token == token);

            if (phienDangNhap != null)
            {
                _context.PhienDangNhaps.Remove(phienDangNhap);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
