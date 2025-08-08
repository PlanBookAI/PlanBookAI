using AuthService.Models.DTOs;
using AuthService.Models.Entities;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/v1/xac-thuc")]
    public class XacThucController : ControllerBase
    {
        private readonly IDichVuJWT _dichVuJWT;
        private readonly ILogger<XacThucController> _logger;

        public XacThucController(IDichVuJWT dichVuJWT, ILogger<XacThucController> logger)
        {
            _dichVuJWT = dichVuJWT;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập và tạo JWT token
        /// </summary>
        /// <param name="yeuCau">Thông tin đăng nhập</param>
        /// <returns>JWT token và thông tin user</returns>
        [HttpPost("dang-nhap")]
        public async Task<ActionResult<PhanHoiDangNhap>> DangNhap([FromBody] YeuCauDangNhap yeuCau)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", yeuCau.Email);

                // Mock user for testing (TODO: Replace with database lookup)
                var mockUser = new NguoiDung
                {
                    Id = Guid.NewGuid(),
                    Email = yeuCau.Email,
                    HoTen = "Test User",
                    MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("123456"),
                    VaiTroId = 4,
                    LaHoatDong = true,
                    LanDangNhapCuoi = DateTime.UtcNow,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };

                var mockRole = new VaiTro
                {
                    Id = 4,
                    Ten = "Teacher",
                    MoTa = "Giáo viên",
                    LaHoatDong = true,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };

                // Verify password (mock: accept "123456")
                if (yeuCau.MatKhau != "123456")
                {
                    _logger.LogWarning("Invalid password for email: {Email}", yeuCau.Email);
                    return Ok(new PhanHoiDangNhap
                    {
                        ThanhCong = false,
                        ThongBao = "Email hoặc mật khẩu không đúng"
                    });
                }

                // Create tokens
                var accessToken = _dichVuJWT.TaoAccessToken(mockUser, mockRole);
                var refreshToken = _dichVuJWT.TaoRefreshToken(mockUser.Id, yeuCau.GhiNho);
                var expiryTime = _dichVuJWT.LayThoiGianHetHan(accessToken);

                var response = new PhanHoiDangNhap
                {
                    ThanhCong = true,
                    ThongBao = "Đăng nhập thành công!",
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    HetHanLuc = expiryTime,
                    NguoiDung = new ThongTinNguoiDung
                    {
                        Id = mockUser.Id,
                        Email = mockUser.Email,
                        HoTen = mockUser.HoTen,
                        VaiTro = mockRole.Ten,
                        LaHoatDong = mockUser.LaHoatDong,
                        LanDangNhapCuoi = mockUser.LanDangNhapCuoi
                    }
                };

                _logger.LogInformation("Login successful for user: {UserId}", mockUser.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", yeuCau.Email);
                return StatusCode(500, new PhanHoiDangNhap
                {
                    ThanhCong = false,
                    ThongBao = "Lỗi hệ thống. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="yeuCau">Thông tin đăng ký</param>
        /// <returns>Thông tin tài khoản đã tạo</returns>
        [HttpPost("dang-ky")]
        public async Task<ActionResult<PhanHoiDangKy>> DangKy([FromBody] YeuCauDangKy yeuCau)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", yeuCau.Email);

                // Mock validation (TODO: Replace with database check)
                if (yeuCau.Email == "existing@test.com")
                {
                    return Ok(new PhanHoiDangKy
                    {
                        ThanhCong = false,
                        ThongBao = "Đăng ký thất bại",
                        LoiValidation = new List<string> { "Email này đã được sử dụng" }
                    });
                }

                // Create mock user (TODO: Save to database)
                var newUserId = Guid.NewGuid();
                var roleName = GetRoleName(yeuCau.VaiTroId);

                var response = new PhanHoiDangKy
                {
                    ThanhCong = true,
                    ThongBao = "Đăng ký thành công! Tài khoản đã được tạo.",
                    NguoiDungId = newUserId,
                    Email = yeuCau.Email,
                    HoTen = yeuCau.HoTen,
                    VaiTro = roleName,
                    LoiValidation = new List<string>()
                };

                _logger.LogInformation("Registration successful for user: {UserId}", newUserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", yeuCau.Email);
                return StatusCode(500, new PhanHoiDangKy
                {
                    ThanhCong = false,
                    ThongBao = "Lỗi hệ thống. Vui lòng thử lại sau.",
                    LoiValidation = new List<string> { "Lỗi hệ thống không xác định" }
                });
            }
        }

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Staff",
                4 => "Teacher",
                _ => "Unknown"
            };
        }
    }
}
