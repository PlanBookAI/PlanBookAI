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

        // GET /api/v1/mau-giao-an - Lấy danh sách mẫu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _service.GetAllAsync(teacherId);
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
        public async Task<IActionResult> Create([FromBody] MauGiaoAn request)
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