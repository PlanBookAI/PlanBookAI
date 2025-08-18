using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services;
using FluentValidation;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/nguoi-dung")]
public class NguoiDungController : ControllerBase
{
	private readonly IDichVuNguoiDung _dichVuNguoiDung;
	private readonly IValidator<YeuCauCapNhatHoSo> _capNhatHoSoValidator;
	private readonly ILogger<NguoiDungController> _logger;

	public NguoiDungController(
		IDichVuNguoiDung dichVuNguoiDung, 
		IValidator<YeuCauCapNhatHoSo> capNhatHoSoValidator,
		ILogger<NguoiDungController> logger)
	{
		_dichVuNguoiDung = dichVuNguoiDung;
		_capNhatHoSoValidator = capNhatHoSoValidator;
		_logger = logger;
	}

	// GET: api/v1/nguoi-dung/ho-so/{userId}
	[HttpGet("ho-so/{userId:guid}")]
	public async Task<ActionResult<PhanHoiHoSo>> LayHoSo(Guid userId)
	{
		var ketQua = await _dichVuNguoiDung.LayHoSoAsync(userId);
		if (!ketQua.ThanhCong)
		{
			return BadRequest(ketQua);
		}
		return Ok(ketQua);
	}

	// PUT: api/v1/nguoi-dung/ho-so/cap-nhat/{userId}
	[HttpPut("ho-so/cap-nhat/{userId:guid}")]
	public async Task<ActionResult<PhanHoiHoSo>> CapNhatHoSo(Guid userId, [FromBody] YeuCauCapNhatHoSo yeuCau)
	{
		try
		{
			_logger.LogInformation("CapNhatHoSo: userId={UserId}, yeuCau={@YeuCau}", userId, yeuCau);

			if (yeuCau == null)
			{
				_logger.LogWarning("CapNhatHoSo: yeuCau là null");
				return BadRequest("Dữ liệu không hợp lệ");
			}

			var validation = await _capNhatHoSoValidator.ValidateAsync(yeuCau);
			_logger.LogInformation("CapNhatHoSo: validation result={@Validation}", validation);

			if (!validation.IsValid)
			{
				_logger.LogWarning("CapNhatHoSo: validation failed");
				return BadRequest(new
				{
					thanhCong = false,
					thongBao = "Dữ liệu không hợp lệ",
					loi = validation.Errors.Select(e => new { truong = e.PropertyName, thongBao = e.ErrorMessage })
				});
			}

			_logger.LogInformation("CapNhatHoSo: validation passed, gọi service");
			var ketQua = await _dichVuNguoiDung.CapNhatHoSoAsync(userId, yeuCau);
			
			if (!ketQua.ThanhCong)
			{
				_logger.LogWarning("CapNhatHoSo: service trả về lỗi");
				return BadRequest(ketQua);
			}

			_logger.LogInformation("CapNhatHoSo: thành công");
			return Ok(ketQua);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CapNhatHoSo: exception {Message}", ex.Message);
			return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
		}
	}

	// GET: api/v1/nguoi-dung/danh-sach
	[HttpGet("danh-sach")]
	public ActionResult<PhanHoiDanhSachNguoiDung> LayDanhSachNguoiDung()
	{
		// TODO: Gateway sẽ xác thực và truyền thông tin user qua header hoặc context
		// Hiện tại chưa có authentication nên trả về 401
		return Unauthorized(new { thanhCong = false, thongBao = "Chưa có xác thực" });
	}

	// GET: api/v1/nguoi-dung/{userId}
	[HttpGet("{userId:guid}")]
	public ActionResult<PhanHoiHoSo> LayThongTinNguoiDung(Guid userId)
	{
		// TODO: Gateway sẽ xác thực và truyền thông tin user qua header hoặc context
		// Hiện tại chưa có authentication nên trả về 401
		return Unauthorized(new { thanhCong = false, thongBao = "Chưa có xác thực" });
	}

	// DELETE: api/v1/nguoi-dung/{userId}
	[HttpDelete("{userId:guid}")]
	public ActionResult<PhanHoiHoSo> XoaNguoiDung(Guid userId)
	{
		// TODO: Gateway sẽ xác thực và truyền thông tin user qua header hoặc context
		// Hiện tại chưa có authentication nên trả về 401
		return Unauthorized(new { thanhCong = false, thongBao = "Chưa có xác thực" });
	}

	// POST: api/v1/nguoi-dung/{userId}/khoi-phuc
	[HttpPost("{userId:guid}/khoi-phuc")]
	public ActionResult<PhanHoiHoSo> KhoiPhucNguoiDung(Guid userId)
	{
		// TODO: Gateway sẽ xác thực và truyền thông tin user qua header hoặc context
		// Hiện tại chưa có authentication nên trả về 401
		return Unauthorized(new { thanhCong = false, thongBao = "Chưa có xác thực" });
	}

	// GET: api/v1/nguoi-dung/kiem-tra/{userId}
	[HttpGet("kiem-tra/{userId:guid}")]
	public async Task<ActionResult<bool>> KiemTraNguoiDungTonTai(Guid userId)
	{
		var tonTai = await _dichVuNguoiDung.KiemTraNguoiDungTonTaiAsync(userId);
		return Ok(tonTai);
	}

	// GET: api/v1/nguoi-dung/kiem-tra-email
	[HttpGet("kiem-tra-email")]
	public async Task<ActionResult<bool>> KiemTraEmailTonTai([FromQuery] string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return BadRequest("Email không được để trống");
		}

		var tonTai = await _dichVuNguoiDung.KiemTraEmailTonTaiAsync(email);
		return Ok(tonTai);
	}
}
