using AuthService.Models.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/v1/xac-thuc")]
    public class XacThucController : ControllerBase
    {
        private readonly IDichVuXacThuc _dichVuXacThuc;
        private readonly ILogger<XacThucController> _logger;

        public XacThucController(IDichVuXacThuc dichVuXacThuc, ILogger<XacThucController> logger)
        {
            _dichVuXacThuc = dichVuXacThuc;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập và tạo JWT token
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập</param>
        /// <returns>JWT token và thông tin user</returns>
        [HttpPost("dang-nhap")]
        public async Task<ActionResult<PhanHoiXacThucDto>> DangNhap([FromBody] DangNhapDto dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

                var phanHoi = await _dichVuXacThuc.TaoToken(dto.Email, dto.MatKhau);

                _logger.LogInformation("Login successful for user: {Email}", dto.Email);
                return Ok(phanHoi);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", dto.Email);
                return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="dto">Thông tin đăng ký</param>
        /// <returns>Thông tin tài khoản đã tạo</returns>
        [HttpPost("dang-ky")]
        public async Task<ActionResult<PhanHoiXacThucDto>> DangKy([FromBody] DangKyDto dto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

                var phanHoi = await _dichVuXacThuc.DangKy(dto.Email, dto.MatKhau, dto.HoTen, dto.VaiTro);

                _logger.LogInformation("Registration successful for user: {Email}", dto.Email);
                return Ok(phanHoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", dto.Email);
                return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Làm mới token truy cập
        /// </summary>
        /// <param name="dto">Refresh token</param>
        /// <returns>Token mới</returns>
        [HttpPost("lam-moi-token")]
        public async Task<ActionResult<PhanHoiXacThucDto>> LamMoiToken([FromBody] LamMoiTokenDto dto)
        {
            try
            {
                _logger.LogInformation("Token refresh attempt");

                var phanHoi = await _dichVuXacThuc.LamMoiToken(dto.RefreshToken);

                _logger.LogInformation("Token refresh successful");
                return Ok(phanHoi);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Invalid refresh token attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Xác thực token truy cập
        /// </summary>
        /// <returns>Email người dùng nếu token hợp lệ</returns>
        [HttpGet("xac-thuc-token")]
        [Authorize]
        public ActionResult<string> XacThucToken()
        {
            try
            {
                // Lấy token từ header Authorization
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token không được cung cấp." });
                }

                var token = authHeader.Substring("Bearer ".Length);
                var email = _dichVuXacThuc.XacThucToken(token);

                if (string.IsNullOrEmpty(email))
                {
                    // Xử lý khi token đã hết hạn hoặc bị giả mạo.
                    return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });
                }

                return Ok(new { email }); // Trả về email người dùng nếu token hợp lệ.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }
    }
}
