using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/lich-su-dang-nhap")]
public class LichSuDangNhapController : ControllerBase
{
    private readonly IDichVuLichSuDangNhap _dichVuLichSuDangNhap;
    private readonly ILogger<LichSuDangNhapController> _logger;

    public LichSuDangNhapController(
        IDichVuLichSuDangNhap dichVuLichSuDangNhap,
        ILogger<LichSuDangNhapController> logger)
    {
        _dichVuLichSuDangNhap = dichVuLichSuDangNhap;
        _logger = logger;
    }

    // POST: api/v1/lich-su-dang-nhap/log
    [HttpPost("log")]
    public async Task<ActionResult<PhanHoiLichSuDangNhap>> LogDangNhap([FromBody] LogDangNhapRequest request)
    {
        try
        {
            var ketQua = await _dichVuLichSuDangNhap.LogDangNhapAsync(request.UserId, request.Token);
            if (!ketQua.ThanhCong)
            {
                return BadRequest(ketQua);
            }
            return Ok(ketQua);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong LogDangNhap: {Message}", ex.Message);
            return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
        }
    }

    // GET: api/v1/lich-su-dang-nhap/{logId}
    [HttpGet("{logId:guid}")]
    public async Task<ActionResult<PhanHoiLichSuDangNhap>> LayLichSuDangNhap(Guid logId)
    {
        try
        {
            var ketQua = await _dichVuLichSuDangNhap.LayLichSuDangNhapAsync(logId);
            if (!ketQua.ThanhCong)
            {
                return NotFound(ketQua);
            }
            return Ok(ketQua);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong LayLichSuDangNhap: {Message}", ex.Message);
            return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
        }
    }

    // GET: api/v1/lich-su-dang-nhap/danh-sach/{userId}
    [HttpGet("danh-sach/{userId:guid}")]
    public async Task<ActionResult<PhanHoiDanhSachLichSuDangNhap>> LayDanhSachLichSuDangNhap(Guid userId)
    {
        try
        {
            var ketQua = await _dichVuLichSuDangNhap.LayDanhSachLichSuDangNhapAsync(userId);
            if (!ketQua.ThanhCong)
            {
                return BadRequest(ketQua);
            }
            return Ok(ketQua);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong LayDanhSachLichSuDangNhap: {Message}", ex.Message);
            return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
        }
    }

    // DELETE: api/v1/lich-su-dang-nhap/{logId}
    [HttpDelete("{logId:guid}")]
    public async Task<ActionResult<PhanHoiLichSuDangNhap>> XoaLichSuDangNhap(Guid logId)
    {
        try
        {
            var ketQua = await _dichVuLichSuDangNhap.XoaLichSuDangNhapAsync(logId);
            if (!ketQua.ThanhCong)
            {
                return BadRequest(ketQua);
            }
            return Ok(ketQua);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong XoaLichSuDangNhap: {Message}", ex.Message);
            return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
        }
    }

    // DELETE: api/v1/lich-su-dang-nhap/user/{userId}
    [HttpDelete("user/{userId:guid}")]
    public async Task<ActionResult<PhanHoiLichSuDangNhap>> XoaLichSuDangNhapCuaUser(Guid userId)
    {
        try
        {
            var ketQua = await _dichVuLichSuDangNhap.XoaLichSuDangNhapCuaUserAsync(userId);
            if (!ketQua.ThanhCong)
            {
                return BadRequest(ketQua);
            }
            return Ok(ketQua);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong XoaLichSuDangNhapCuaUser: {Message}", ex.Message);
            return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
        }
    }
}

// DTO cho request log đăng nhập
public class LogDangNhapRequest
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}
