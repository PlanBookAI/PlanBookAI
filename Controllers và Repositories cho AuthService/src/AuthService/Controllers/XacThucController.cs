using AuthService.Models.DTOs;
using AuthService.Services.DichVuXacThuc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    /// <summary>
    /// Controller xử lý các yêu cầu liên quan đến xác thực người dùng.
    /// </summary>
    [ApiController]
    [Route("api/v1/xac-thuc")] // Định tuyến URL cho controller này.
    public class XacThucController : ControllerBase
    {
        private readonly IDichVuXacThuc _dichVuXacThuc;

        public XacThucController(IDichVuXacThuc dichVuXacThuc)
        {
            _dichVuXacThuc = dichVuXacThuc;
        }

        /// <summary>
        /// Xử lý yêu cầu đăng nhập và tạo token.
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập từ client.</param>
        /// <returns>Access Token và Refresh Token nếu thành công.</returns>
        [HttpPost("dang-nhap")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapDto dto)
        {
            try
            {
                var phanHoi = await _dichVuXacThuc.TaoToken(dto.TenDangNhap, dto.MatKhau);
                return Ok(phanHoi); // Trả về mã 200 OK cùng với token.
            }
            catch (UnauthorizedAccessException ex)
            {
                // Xử lý lỗi đăng nhập không hợp lệ.
                return Unauthorized(new { message = ex.Message }); // Trả về mã 401 Unauthorized.
            }
        }

        /// <summary>
        /// Xử lý yêu cầu làm mới Access Token bằng Refresh Token.
        /// </summary>
        /// <param name="dto">Refresh Token từ client.</param>
        /// <returns>Cặp token mới nếu thành công.</returns>
        [HttpPost("lam-moi-token")]
        public async Task<IActionResult> LamMoiToken([FromBody] LamMoiTokenDto dto)
        {
            try
            {
                var phanHoi = await _dichVuXacThuc.LamMoiToken(dto.RefreshToken);
                return Ok(phanHoi);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Xử lý lỗi refresh token không hợp lệ.
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xử lý yêu cầu xác thực Access Token.
        /// </summary>
        /// <param name="authorizationHeader">Token từ header Authorization.</param>
        /// <returns>Thông tin người dùng nếu token hợp lệ.</returns>
        [HttpGet("xac-thuc-token")]
        public IActionResult XacThucToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token truy cập không hợp lệ." });
            }

            var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tenDangNhap = _dichVuXacThuc.XacThucToken(accessToken);

            if (tenDangNhap == null)
            {
                // Xử lý khi token đã hết hạn hoặc bị giả mạo.
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            return Ok(new { tenDangNhap }); // Trả về thông tin người dùng nếu token hợp lệ.
        }
    }
}