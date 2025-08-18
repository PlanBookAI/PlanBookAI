using AutoMapper;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Repositories;

namespace UserService.Services;

public class DichVuLichSuDangNhap : IDichVuLichSuDangNhap
{
    private readonly ILichSuDangNhapRepository _lichSuDangNhapRepository;
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DichVuLichSuDangNhap> _logger;

    public DichVuLichSuDangNhap(
        ILichSuDangNhapRepository lichSuDangNhapRepository,
        INguoiDungRepository nguoiDungRepository,
        IMapper mapper,
        ILogger<DichVuLichSuDangNhap> logger)
    {
        _lichSuDangNhapRepository = lichSuDangNhapRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PhanHoiLichSuDangNhap> LogDangNhapAsync(Guid userId, string token)
    {
        try
        {
            // Kiểm tra user có tồn tại không
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(userId);
            if (nguoiDung == null)
            {
                return new PhanHoiLichSuDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Người dùng không tồn tại"
                };
            }

            // Log đăng nhập
            var lichSuDangNhap = new LichSuDangNhap
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(1), // Không set Kind vì database dùng timestamp without time zone
                CreatedAt = DateTime.UtcNow // Không set Kind vì database dùng timestamp without time zone
            };

            var logCreated = await _lichSuDangNhapRepository.CreateAsync(lichSuDangNhap);
            var logDto = _mapper.Map<ThongTinLichSuDangNhapDto>(logCreated);

            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = true,
                ThongBao = "Log đăng nhập thành công",
                DuLieu = logDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi log đăng nhập cho người dùng {UserId}", userId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi log đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiLichSuDangNhap> LayLichSuDangNhapAsync(Guid logId)
    {
        try
        {
            var lichSu = await _lichSuDangNhapRepository.GetByIdAsync(logId);
            if (lichSu == null)
            {
                return new PhanHoiLichSuDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Log đăng nhập không tồn tại"
                };
            }

            var logDto = _mapper.Map<ThongTinLichSuDangNhapDto>(lichSu);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = true,
                ThongBao = "Lấy thông tin log đăng nhập thành công",
                DuLieu = logDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin log đăng nhập {LogId}", logId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi lấy thông tin log đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiDanhSachLichSuDangNhap> LayDanhSachLichSuDangNhapAsync(Guid userId)
    {
        try
        {
            var lichSuList = await _lichSuDangNhapRepository.GetByUserIdAsync(userId);
            var logDtos = _mapper.Map<List<ThongTinLichSuDangNhapDto>>(lichSuList);

            return new PhanHoiDanhSachLichSuDangNhap
            {
                ThanhCong = true,
                ThongBao = "Lấy danh sách lịch sử đăng nhập thành công",
                DuLieu = logDtos,
                TongSo = logDtos.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách lịch sử đăng nhập cho người dùng {UserId}", userId);
            return new PhanHoiDanhSachLichSuDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi lấy danh sách lịch sử đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiLichSuDangNhap> XoaLichSuDangNhapAsync(Guid logId)
    {
        try
        {
            await _lichSuDangNhapRepository.DeleteAsync(logId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = true,
                ThongBao = "Xóa log đăng nhập thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa log đăng nhập {LogId}", logId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi xóa log đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<PhanHoiLichSuDangNhap> XoaLichSuDangNhapCuaUserAsync(Guid userId)
    {
        try
        {
            await _lichSuDangNhapRepository.DeleteByUserIdAsync(userId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = true,
                ThongBao = "Xóa lịch sử đăng nhập của user thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa lịch sử đăng nhập của user {UserId}", userId);
            return new PhanHoiLichSuDangNhap
            {
                ThanhCong = false,
                ThongBao = $"Lỗi khi xóa lịch sử đăng nhập của user: {ex.Message}"
            };
        }
    }
}
