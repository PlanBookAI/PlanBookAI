using AutoMapper;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Repositories;
using .Net; // Thư viện để mã hóa mật khẩu và OTP

namespace UserService.Services;

public class DichVuNguoiDung : IDichVuNguoiDung
{
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IVaiTroRepository _vaiTroRepository; // Có thể không cần dùng tới
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

    // Các phương thức đã có sẵn
    public async Task<PhanHoiHoSo> LayHoSoAsync(Guid userId)
    {
        try
        {
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(userId);
            if (nguoiDung == null)
            {
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Không tìm thấy người dùng" };
            }

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Lấy hồ sơ thành công", DuLieu = hoSoDto };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = $"Lỗi khi lấy hồ sơ: {ex.Message}" };
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
                nguoiDung.HoSoNguoiDung = _mapper.Map<HoSoNguoiDung>(yeuCau);
            }
            else
            {
                _mapper.Map(yeuCau, nguoiDung.HoSoNguoiDung);
            }

            await _nguoiDungRepository.UpdateAsync(nguoiDung);

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Cập nhật hồ sơ thành công", DuLieu = hoSoDto };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = $"Lỗi khi cập nhật hồ sơ: {ex.Message}" };
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
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Không tìm thấy người dùng" };
            }

            var hoSoDto = _mapper.Map<ThongTinHoSoDto>(nguoiDung);
            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Lấy thông tin người dùng thành công", DuLieu = hoSoDto };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = $"Lỗi khi lấy thông tin: {ex.Message}" };
        }
    }

    public async Task<PhanHoiHoSo> XoaNguoiDungAsync(Guid adminId, Guid userId)
    {
        try
        {
            if (adminId == userId)
            {
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Không thể xóa chính mình" };
            }

            var success = await _nguoiDungRepository.SoftDeleteAsync(userId);
            if (!success)
            {
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Không tìm thấy người dùng để xóa" };
            }

            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Xóa người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = $"Lỗi khi xóa người dùng: {ex.Message}" };
        }
    }

    public async Task<PhanHoiHoSo> KhoiPhucNguoiDungAsync(Guid adminId, Guid userId)
    {
        try
        {
            var success = await _nguoiDungRepository.RestoreAsync(userId);
            if (!success)
            {
                return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Không tìm thấy người dùng để khôi phục" };
            }

            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Khôi phục người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = $"Lỗi khi khôi phục người dùng: {ex.Message}" };
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

    // --- BỔ SUNG: Các phương thức cho tính năng Quên Mật khẩu ---

    public async Task<PhanHoiHoSo> QuenMatKhauAsync(string email)
    {
        var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(email);
        if (nguoiDung == null)
        {
            // Trả về thông báo chung chung để tránh tấn công user enumeration
            return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Nếu email tồn tại, một mã OTP đã được gửi." };
        }

        // Tạo OTP và thời gian hết hạn (15 phút)
        var otp = OtpGenerator.GenerateRandomOtp(6);
        var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);

        var otpCode = new OtpCode
        {
            UserId = nguoiDung.Id,
            OtpHash = otpHash,
            ExpiresAt = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(15), DateTimeKind.Utc)
        };

        await _nguoiDungRepository.SaveOtpCodeAsync(otpCode);

        // Giả định bạn có lớp EmailSender để gửi email
        await EmailSender.SendEmailAsync(
            email,
            "Mã OTP đặt lại mật khẩu của bạn",
            $"Mã OTP của bạn là: {otp}. Mã này sẽ hết hạn sau 15 phút."
        );

        return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Mã OTP đã được gửi đến email của bạn." };
    }

    public async Task<PhanHoiHoSo> XacThucOtpAsync(string email, string otp)
    {
        var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(email);
        if (nguoiDung == null)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Email hoặc OTP không hợp lệ." };
        }

        var otpFromDb = await _nguoiDungRepository.GetValidOtpByUserIdAndHashAsync(nguoiDung.Id, BCrypt.Net.BCrypt.HashPassword(otp));

        if (otpFromDb == null)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Email hoặc OTP không hợp lệ, hoặc đã hết hạn." };
        }

        await _nguoiDungRepository.InvalidateOtpAsync(otpFromDb.Id);

        return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Xác thực OTP thành công." };
    }

    public async Task<PhanHoiHoSo> DatLaiMatKhauAsync(string email, string otp, string matKhauMoi)
    {
        var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(email);
        if (nguoiDung == null)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Yêu cầu không hợp lệ." };
        }

        // Bước 1: Xác thực lại OTP để đảm bảo quy trình
        var otpFromDb = await _nguoiDungRepository.GetValidOtpByUserIdAndHashAsync(nguoiDung.Id, BCrypt.Net.BCrypt.HashPassword(otp));
        if (otpFromDb == null)
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Mã OTP không hợp lệ, hoặc đã hết hạn." };
        }

        // Bước 2: Kiểm tra mật khẩu mới có trùng với mật khẩu cũ không
        var oldPasswordHistory = await _nguoiDungRepository.GetPasswordHistoryByUserIdAsync(nguoiDung.Id);
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
        if (oldPasswordHistory.Any(h => BCrypt.Net.BCrypt.Verify(matKhauMoi, h.PasswordHash)))
        {
            return new PhanHoiHoSo { ThanhCong = false, ThongBao = "Mật khẩu mới không được trùng với các mật khẩu cũ." };
        }

        // Bước 3: Lưu lại mật khẩu cũ vào lịch sử
        if (!string.IsNullOrEmpty(nguoiDung.PasswordHash))
        {
            var history = new PasswordHistory
            {
                UserId = nguoiDung.Id,
                PasswordHash = nguoiDung.PasswordHash,
                ChangedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };
            await _nguoiDungRepository.SavePasswordHistoryAsync(history);
        }

        // Bước 4: Cập nhật mật khẩu mới và lưu vào DB
        nguoiDung.PasswordHash = newPasswordHash;
        await _nguoiDungRepository.UpdateAsync(nguoiDung);

        // Hủy OTP sau khi đặt lại mật khẩu thành công
        await _nguoiDungRepository.InvalidateOtpAsync(otpFromDb.Id);

        return new PhanHoiHoSo { ThanhCong = true, ThongBao = "Đặt lại mật khẩu thành công." };
    }
}