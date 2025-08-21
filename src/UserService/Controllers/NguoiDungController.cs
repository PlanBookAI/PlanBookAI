using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services;
using UserService.Extensions;
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
	public async Task<ActionResult<PhanHoiDanhSachNguoiDung>> LayDanhSachNguoiDung()
	{
		try
		{
			// Debug: Log tất cả thông tin authentication
			_logger.LogInformation("LayDanhSachNguoiDung: Bắt đầu xử lý request");
			_logger.LogInformation("LayDanhSachNguoiDung: IsAuthenticated = {IsAuthenticated}", HttpContext.IsAuthenticated());
			_logger.LogInformation("LayDanhSachNguoiDung: UserId = {UserId}", HttpContext.GetUserId());
			_logger.LogInformation("LayDanhSachNguoiDung: UserRole = {UserRole}", HttpContext.GetUserRole());
			_logger.LogInformation("LayDanhSachNguoiDung: UserEmail = {UserEmail}", HttpContext.GetUserEmail());

			// Kiểm tra authentication
			if (!HttpContext.IsAuthenticated())
			{
				_logger.LogWarning("LayDanhSachNguoiDung: Không có xác thực");
				return Unauthorized(new { thanhCong = false, thongBao = "Không có xác thực" });
			}

			// Kiểm tra quyền ADMIN
			if (!HttpContext.IsAdmin())
			{
				_logger.LogWarning("LayDanhSachNguoiDung: Không có quyền ADMIN, UserRole: {UserRole}", HttpContext.GetUserId());
				return Unauthorized(new { thanhCong = false, thongBao = "Không có quyền ADMIN" });
			}

			var adminId = Guid.Parse(HttpContext.GetUserId()!);
			_logger.LogInformation("LayDanhSachNguoiDung: Admin {AdminId} yêu cầu xem danh sách người dùng", adminId);

			var ketQua = await _dichVuNguoiDung.LayDanhSachNguoiDungAsync(adminId);
			if (!ketQua.ThanhCong)
			{
				_logger.LogWarning("LayDanhSachNguoiDung: Service trả về lỗi");
				return BadRequest(ketQua);
			}

			_logger.LogInformation("LayDanhSachNguoiDung: Thành công, trả về {Count} người dùng", 
								 ketQua.DanhSach?.Count ?? 0);
			return Ok(ketQua);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "LayDanhSachNguoiDung: Exception {Message}", ex.Message);
			return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
		}
	}

	// GET: api/v1/nguoi-dung/{userId}
	[HttpGet("{userId:guid}")]
	public async Task<ActionResult<PhanHoiHoSo>> LayThongTinNguoiDung(Guid userId)
	{
		try
		{
			// Kiểm tra authentication
			if (!HttpContext.IsAuthenticated())
			{
				_logger.LogWarning("LayThongTinNguoiDung: Không có xác thực");
				return Unauthorized(new { thanhCong = false, thongBao = "Không có xác thực" });
			}

			// Kiểm tra quyền ADMIN hoặc chính user đó
			var currentUserId = Guid.Parse(HttpContext.GetUserId()!);
			var currentUserRole = HttpContext.GetUserRole();

			if (!HttpContext.IsAdmin() && currentUserId != userId)
			{
				_logger.LogWarning("LayThongTinNguoiDung: Không có quyền xem thông tin user khác, UserRole: {UserRole}, CurrentUserId: {CurrentUserId}, RequestedUserId: {RequestedUserId}", 
								 currentUserRole, currentUserId, userId);
				return Unauthorized(new { thanhCong = false, thongBao = "Không có quyền xem thông tin user khác" });
			}

			_logger.LogInformation("LayThongTinNguoiDung: User {CurrentUserId} yêu cầu xem thông tin user {RequestedUserId}", 
								 currentUserId, userId);

			var ketQua = await _dichVuNguoiDung.LayThongTinNguoiDungAsync(currentUserId, userId);
			if (!ketQua.ThanhCong)
			{
				_logger.LogWarning("LayThongTinNguoiDung: Service trả về lỗi");
				return BadRequest(ketQua);
			}

			_logger.LogInformation("LayThongTinNguoiDung: Thành công");
			return Ok(ketQua);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "LayThongTinNguoiDung: Exception {Message}", ex.Message);
			return StatusCode(500, new { thanhCong = false, thongBao = "Lỗi server" });
		}
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

    /// <summary>
    /// Gửi mã OTP để đặt lại mật khẩu cho người dùng.
    /// </summary>
    [HttpPost("quen-mat-khau")]
    [ProducesResponseType(typeof(PhanHoiHoSo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> QuenMatKhau([FromBody] YeuCauQuenMatKhau yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var phanHoi = await _dichVuNguoiDung.QuenMatKhauAsync(yeuCau.Email);
        return Ok(phanHoi);
    }

    /// <summary>
    /// Xác thực mã OTP để cho phép người dùng đặt mật khẩu mới.
    /// </summary>
    [HttpPost("xac-thuc-otp")]
    [ProducesResponseType(typeof(PhanHoiHoSo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XacThucOtp([FromBody] YeuCauXacThucOtpDto yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var phanHoi = await _dichVuNguoiDung.XacThucOtpAsync(yeuCau.Email, yeuCau.Otp);
        if (!phanHoi.ThanhCong)
        {
            return BadRequest(phanHoi);
        }
        return Ok(phanHoi);
    }

    /// <summary>
    /// Đặt lại mật khẩu mới sau khi xác thực OTP thành công.
    /// </summary>
    [HttpPost("dat-lai-mat-khau")]
    [ProducesResponseType(typeof(PhanHoiHoSo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DatLaiMatKhau([FromBody] YeuCauDatLaiMatKhauDto yeuCau)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var phanHoi = await _dichVuNguoiDung.DatLaiMatKhauAsync(yeuCau.Email, yeuCau.Otp, yeuCau.MatKhauMoi);
        if (!phanHoi.ThanhCong)
        {
            return BadRequest(phanHoi);
        }
        return Ok(phanHoi);
    }

    // --- Endpoint mới để xem lịch sử mật khẩu ---

    /// <summary>
    /// Xem lịch sử thay đổi mật khẩu của một người dùng cụ thể.
    /// (Lưu ý: Cần xác thực quyền admin hoặc chính người dùng đó)
    /// </summary>
    [HttpGet("lich-su-mat-khau/{userId}")]
    [ProducesResponseType(typeof(PhanHoiDanhSachLichSuDangNhap), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XemLichSuMatKhau(Guid userId)
    {
        return Ok(new PhanHoiHoSo { ThanhCong = true, ThongBao = "Tính năng xem lịch sử mật khẩu đang được phát triển." });
    }
}
