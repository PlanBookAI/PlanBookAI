using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Extensions;
using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Services;

namespace PlanService.Controllers
{
    [ApiController]
    [Route("api/v1/mau-giao-an")]
    [Authorize(Policy = "TeacherOnly")]
    public class MauGiaoAnController : ControllerBase
    {
        private readonly IMauGiaoAnService _service;

        public MauGiaoAnController(IMauGiaoAnService service)
        {
            _service = service;
        }

        // GET /api/v1/mau-giao-an - Deprecated endpoint (giữ tạm theo spec)
        [HttpGet]
        public IActionResult GetDeprecated()
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                thanhCong = false,
                thongDiep = "Endpoint này đã bị thay thế. Vui lòng sử dụng /api/v1/mau-giao-an/cong-khai hoặc /api/v1/mau-giao-an/cua-toi"
            });
        }

        // GET /api/v1/mau-giao-an/cong-khai - Danh sách mẫu công khai
        [HttpGet("cong-khai")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublic([FromQuery] string? keyword, [FromQuery] string? monHoc, [FromQuery] int? khoi)
        {
            var rs = await _service.LayCongKhaiAsync(keyword, monHoc, khoi);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/mau-giao-an/cua-toi - Danh sách mẫu do tôi tạo
        [HttpGet("cua-toi")]
        public async Task<IActionResult> GetMine([FromQuery] string? keyword, [FromQuery] string? monHoc, [FromQuery] int? khoi)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _service.LayCuaToiAsync(teacherId, keyword, monHoc, khoi);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/mau-giao-an/{id} - Lấy chi tiết mẫu
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _service.GetByIdAsync(id, teacherId);
            return rs.ThanhCong && rs.DuLieu != null ? Ok(rs) : NotFound(rs);
        }

        // POST /api/v1/mau-giao-an - Tạo mẫu mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] YeuCauTaoMauGiaoAn request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<MauGiaoAn>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var teacherId = HttpContext.GetUserId();
            var rs = await _service.CreateAsync(request, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // PUT /api/v1/mau-giao-an/{id} - Cập nhật mẫu
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauGiaoAn request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<MauGiaoAn>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var teacherId = HttpContext.GetUserId();
            var rs = await _service.UpdateAsync(id, request, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // DELETE /api/v1/mau-giao-an/{id} - Xóa mẫu
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _service.DeleteAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : NotFound(rs);
        }

        // POST /api/v1/mau-giao-an/{id}/chia-se - Chia sẻ mẫu
        [HttpPost("{id:guid}/chia-se")]
        public async Task<IActionResult> Share(Guid id, [FromQuery] bool chiaSe = true)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _service.ChiaSeAsync(id, chiaSe, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }
    }
}