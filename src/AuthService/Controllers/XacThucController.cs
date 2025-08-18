using Microsoft.AspNetCore.Mvc;
using AuthService.Models.DTOs;
using AuthService.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/v1/xac-thuc")]
public class XacThucController : ControllerBase
{
    private readonly IDichVuXacThuc _dichVuXacThuc;

    public XacThucController(IDichVuXacThuc dichVuXacThuc)
    {
        _dichVuXacThuc = dichVuXacThuc;
    }

    /// <summary>
    /// Đăng nhập người dùng
    /// </summary>
    [HttpPost("dang-nhap")]
    public async Task<ActionResult<PhanHoiDangNhap>> DangNhap([FromBody] YeuCauDangNhap yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ketQua = await _dichVuXacThuc.DangNhapAsync(yeuCau);
        
        if (ketQua.ThanhCong)
        {
            return Ok(ketQua);
        }

        return Unauthorized(ketQua);
    }

    /// <summary>
    /// Đăng ký người dùng mới
    /// </summary>
    [HttpPost("dang-ky")]
    public async Task<ActionResult<PhanHoiDangKy>> DangKy([FromBody] YeuCauDangKy yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ketQua = await _dichVuXacThuc.DangKyAsync(yeuCau);
        
        if (ketQua.ThanhCong)
        {
            return CreatedAtAction(nameof(DangKy), ketQua);
        }

        return BadRequest(ketQua);
    }

    /// <summary>
    /// Làm mới access token
    /// </summary>
    [HttpPost("lam-moi-token")]
    public async Task<ActionResult<PhanHoiXacThucDto>> LamMoiToken([FromBody] LamMoiTokenDto yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ketQua = await _dichVuXacThuc.LamMoiTokenAsync(yeuCau.RefreshToken);
        
        if (ketQua.ThanhCong)
        {
            return Ok(ketQua);
        }

        return Unauthorized(ketQua);
    }

    /// <summary>
    /// Đăng xuất người dùng
    /// </summary>
    [HttpPost("dang-xuat")]
    public async Task<ActionResult> DangXuat([FromBody] LamMoiTokenDto yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ketQua = await _dichVuXacThuc.DangXuatAsync(yeuCau.RefreshToken);
        
        if (ketQua)
        {
            return Ok(new { ThongBao = "Đăng xuất thành công" });
        }

        return BadRequest(new { ThongBao = "Lỗi đăng xuất" });
    }
}
