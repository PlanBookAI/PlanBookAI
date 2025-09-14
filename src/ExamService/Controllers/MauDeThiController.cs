using ExamService.Interfaces;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.Controllers
{
    /// <summary>
    /// Controller quản lý mẫu đề thi
    /// </summary>
    [ApiController]
    [Route("api/v1/mau-de-thi")]
    [Authorize]
    public class MauDeThiController : ControllerBase
    {
        private readonly IMauDeThiService _mauDeThiService;
        private readonly ILogger<MauDeThiController> _logger;

        public MauDeThiController(IMauDeThiService mauDeThiService, ILogger<MauDeThiController> logger)
        {
            _mauDeThiService = mauDeThiService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách mẫu đề thi với phân trang và lọc
        /// </summary>
        /// <param name="pageNumber">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng mỗi trang (mặc định: 10)</param>
        /// <param name="monHoc">Lọc theo môn học</param>
        /// <param name="khoiLop">Lọc theo khối lớp</param>
        /// <param name="trangThai">Lọc theo trạng thái</param>
        /// <returns>Danh sách mẫu đề thi</returns>
        [HttpGet]
        public async Task<ActionResult<ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>>> LayDanhSachMauDeThi(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? monHoc = null,
            [FromQuery] int? khoiLop = null,
            [FromQuery] string? trangThai = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mauDeThiService.LayDanhSachMauDeThiAsync(
                    userId, pageNumber, pageSize, monHoc, khoiLop, trangThai);

                return Ok(ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>.ThanhCong(result, "Lấy danh sách mẫu đề thi thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách mẫu đề thi");
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi lấy danh sách mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Lấy chi tiết mẫu đề thi theo ID
        /// </summary>
        /// <param name="id">ID của mẫu đề thi</param>
        /// <returns>Chi tiết mẫu đề thi</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiPhanHoi<MauDeThiResponseDTO>>> LayChiTietMauDeThi(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mauDeThiService.LayChiTietMauDeThiAsync(id, userId);

                if (result == null)
                {
                    return NotFound(ApiPhanHoi<object>.ThatBai("Không tìm thấy mẫu đề thi", 404));
                }

                return Ok(ApiPhanHoi<MauDeThiResponseDTO>.ThanhCong(result, "Lấy chi tiết mẫu đề thi thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết mẫu đề thi {MauDeThiId}", id);
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi lấy chi tiết mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Tạo mẫu đề thi mới
        /// </summary>
        /// <param name="request">Thông tin mẫu đề thi</param>
        /// <returns>Mẫu đề thi đã tạo</returns>
        [HttpPost]
        public async Task<ActionResult<ApiPhanHoi<MauDeThiResponseDTO>>> TaoMauDeThi([FromBody] MauDeThiRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
                }

                var userId = GetCurrentUserId();
                var result = await _mauDeThiService.TaoMauDeThiAsync(request, userId);

                return CreatedAtAction(
                    nameof(LayChiTietMauDeThi),
                    new { id = result.Id },
                    ApiPhanHoi<MauDeThiResponseDTO>.ThanhCong(result, "Tạo mẫu đề thi thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai(ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mẫu đề thi");
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi tạo mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Cập nhật mẫu đề thi
        /// </summary>
        /// <param name="id">ID của mẫu đề thi</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Mẫu đề thi đã cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiPhanHoi<MauDeThiResponseDTO>>> CapNhatMauDeThi(Guid id, [FromBody] MauDeThiRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
                }

                var userId = GetCurrentUserId();
                var result = await _mauDeThiService.CapNhatMauDeThiAsync(id, request, userId);

                return Ok(ApiPhanHoi<MauDeThiResponseDTO>.ThanhCong(result, "Cập nhật mẫu đề thi thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai(ex.Message, 400));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiPhanHoi<object>.ThatBai("Không tìm thấy mẫu đề thi", 404));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mẫu đề thi {MauDeThiId}", id);
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi cập nhật mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Xóa mẫu đề thi
        /// </summary>
        /// <param name="id">ID của mẫu đề thi</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiPhanHoi<object>>> XoaMauDeThi(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _mauDeThiService.XoaMauDeThiAsync(id, userId);

                if (!success)
                {
                    return NotFound(ApiPhanHoi<object>.ThatBai("Không tìm thấy mẫu đề thi", 404));
                }

                return Ok(ApiPhanHoi<object>.ThanhCong(null, "Xóa mẫu đề thi thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai(ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mẫu đề thi {MauDeThiId}", id);
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi xóa mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Sao chép mẫu đề thi
        /// </summary>
        /// <param name="id">ID của mẫu đề thi gốc</param>
        /// <param name="request">Thông tin sao chép</param>
        /// <returns>Mẫu đề thi đã sao chép</returns>
        [HttpPost("{id}/sao-chep")]
        public async Task<ActionResult<ApiPhanHoi<MauDeThiResponseDTO>>> SaoChepMauDeThi(Guid id, [FromBody] SaoChepMauDeThiDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
                }

                var userId = GetCurrentUserId();
                var result = await _mauDeThiService.SaoChepMauDeThiAsync(id, request, userId);

                return CreatedAtAction(
                    nameof(LayChiTietMauDeThi),
                    new { id = result.Id },
                    ApiPhanHoi<MauDeThiResponseDTO>.ThanhCong(result, "Sao chép mẫu đề thi thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai(ex.Message, 400));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiPhanHoi<object>.ThatBai("Không tìm thấy mẫu đề thi", 404));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sao chép mẫu đề thi {MauDeThiId}", id);
                return StatusCode(500, ApiPhanHoi<object>.ThatBai("Có lỗi xảy ra khi sao chép mẫu đề thi", 500));
            }
        }

        /// <summary>
        /// Lấy ID của user hiện tại từ header
        /// </summary>
        /// <returns>User ID</returns>
        private Guid GetCurrentUserId()
        {
            var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }
            return userId;
        }
    }
}