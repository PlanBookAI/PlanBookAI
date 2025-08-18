using AutoMapper;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Repositories;

namespace UserService.Services;

public class DichVuNguoiDung : IDichVuNguoiDung
{
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IVaiTroRepository _vaiTroRepository;
    private readonly IMapper _mapper;

    public DichVuNguoiDung(
        INguoiDungRepository nguoiDungRepository,
        IVaiTroRepository vaiTroRepository,
        IMapper mapper)
    {
        _nguoiDungRepository = nguoiDungRepository;
        _vaiTroRepository = vaiTroRepository;
        _mapper = mapper;
    }

    public async Task<PhanHoiHoSo> LayHoSoAsync(Guid userId)
    {
        try
        {
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(userId);
            if (nguoiDung == null)
            {
                return new PhanHoiHoSo
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy người dùng"
                };
            }

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Lấy hồ sơ thành công",
                DuLieu = hoSoDto
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi lấy hồ sơ: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiHoSo> CapNhatHoSoAsync(Guid userId, YeuCauCapNhatHoSo yeuCau)
    {
        try
        {
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(userId);
            if (nguoiDung == null)
            {
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Người dùng không tồn tại" };
            }

            // Cập nhật hoặc tạo mới profile
            if (nguoiDung.HoSoNguoiDung == null)
            {
                // Tạo mới profile
                nguoiDung.HoSoNguoiDung = new HoSoNguoiDung
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    HoTen = yeuCau.HoTen,
                    SoDienThoai = yeuCau.SoDienThoai,
                    DiaChi = yeuCau.DiaChi,
                    MoTaBanThan = yeuCau.MoTaBanThan,
                    AnhDaiDienUrl = yeuCau.AnhDaiDienUrl,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };
            }
            else
            {
                // Cập nhật profile hiện có
                nguoiDung.HoSoNguoiDung.HoTen = yeuCau.HoTen;
                nguoiDung.HoSoNguoiDung.SoDienThoai = yeuCau.SoDienThoai;
                nguoiDung.HoSoNguoiDung.DiaChi = yeuCau.DiaChi;
                nguoiDung.HoSoNguoiDung.MoTaBanThan = yeuCau.MoTaBanThan;
                nguoiDung.HoSoNguoiDung.AnhDaiDienUrl = yeuCau.AnhDaiDienUrl;
                nguoiDung.HoSoNguoiDung.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            }

            // Cập nhật thời gian cập nhật của user
            nguoiDung.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _nguoiDungRepository.UpdateAsync(nguoiDung);

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Cập nhật hồ sơ thành công",
                DuLieu = hoSoDto
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi cập nhật hồ sơ: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiDanhSachNguoiDung> LayDanhSachNguoiDungAsync(Guid adminId)
    {
        try
        {
            var nguoiDungList = await _nguoiDungRepository.GetAllExceptAsync(adminId);
            var danhSachDto = nguoiDungList.Select(n => _mapper.Map<ThongTinNguoiDungDto>(n)).ToList();

            return new PhanHoiDanhSachNguoiDung
            {
                ThanhCong = true,
                ThongBao = "Lấy danh sách người dùng thành công",
                DanhSach = danhSachDto,
                TongSo = danhSachDto.Count
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiDanhSachNguoiDung
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi lấy danh sách: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiHoSo> LayThongTinNguoiDungAsync(Guid adminId, Guid userId)
    {
        try
        {
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(userId);
            if (nguoiDung == null)
            {
                return new PhanHoiHoSo
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy người dùng"
                };
            }

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Lấy thông tin người dùng thành công",
                DuLieu = hoSoDto
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi lấy thông tin: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiHoSo> XoaNguoiDungAsync(Guid adminId, Guid userId)
    {
        try
        {
            if (adminId == userId)
            {
                return new PhanHoiHoSo
                {
                    ThanhCong = false,
                    ThongBao = "Không thể xóa chính mình"
                };
            }

            var success = await _nguoiDungRepository.SoftDeleteAsync(userId);
            if (!success)
            {
                return new PhanHoiHoSo
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy người dùng để xóa"
                };
            }

            return new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Xóa người dùng thành công"
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi xóa người dùng: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiHoSo> KhoiPhucNguoiDungAsync(Guid adminId, Guid userId)
    {
        try
        {
            var success = await _nguoiDungRepository.RestoreAsync(userId);
            if (!success)
            {
                return new PhanHoiHoSo
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy người dùng để khôi phục"
                };
            }

            return new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Khôi phục người dùng thành công"
            };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi khôi phục người dùng: {ex.Message}"
            };
        }
    }

    public async Task<bool> KiemTraNguoiDungTonTaiAsync(Guid userId)
    {
        return await _nguoiDungRepository.ExistsAsync(userId);
    }

    public async Task<bool> KiemTraEmailTonTaiAsync(string email)
    {
        var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(email);
        return nguoiDung != null;
    }
}
