using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Extensions;
using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Services;

namespace PlanService.Controllers
{
    [ApiController]
    [Route("api/v1/giao-an")]
    [Authorize(Policy = "TeacherOnly")]
    public class GiaoAnController : ControllerBase
    {
        private readonly IGiaoAnService _giaoAnService;

        public GiaoAnController(IGiaoAnService giaoAnService)
        {
            _giaoAnService = giaoAnService;
        }

        // GET /api/v1/giao-an - Lấy danh sách giáo án (theo teacher_id)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.GetAllAsync(teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/giao-an/{id} - Lấy chi tiết giáo án (theo teacher_id)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.GetByIdAsync(id, teacherId);
            return rs.ThanhCong && rs.DuLieu != null ? Ok(rs) : NotFound(rs);
        }

        // POST /api/v1/giao-an - Tạo giáo án mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] YeuCauTaoGiaoAn request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<GiaoAn>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.CreateAsync(request, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // PUT /api/v1/giao-an/{id} - Cập nhật giáo án
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] YeuCauTaoGiaoAn request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<GiaoAn>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.UpdateAsync(id, request, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // DELETE /api/v1/giao-an/{id} - Xóa giáo án
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.DeleteAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : NotFound(rs);
        }

        // POST /api/v1/giao-an/{id}/phe-duyet - Phê duyệt giáo án
        [HttpPost("{id:guid}/phe-duyet")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.PheDuyetAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // POST /api/v1/giao-an/{id}/xuat-ban - Xuất bản giáo án
        [HttpPost("{id:guid}/xuat-ban")]
        public async Task<IActionResult> Publish(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.XuatBanAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // POST /api/v1/giao-an/{id}/luu-tru - Lưu trữ giáo án
        [HttpPost("{id:guid}/luu-tru")]
        public async Task<IActionResult> Archive(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.LuuTruAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // POST /api/v1/giao-an/{id}/copy - Sao chép giáo án
        [HttpPost("{id:guid}/copy")]
        public async Task<IActionResult> Copy(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.CopyAsync(id, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/giao-an/tim-kiem - Tìm kiếm giáo án (theo keyword)
        [HttpGet("tim-kiem")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.TimKiemAsync(keyword ?? string.Empty, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/giao-an/loc-theo-chu-de/{chuDeId} - Lọc theo chủ đề
        [HttpGet("loc-theo-chu-de/{chuDeId:guid}")]
        public async Task<IActionResult> FilterByTopic(Guid chuDeId)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.LocTheoChuDeAsync(chuDeId, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/giao-an/loc-theo-mon/{monHoc} - Lọc theo môn học
        [HttpGet("loc-theo-mon/{monHoc}")]
        public async Task<IActionResult> FilterBySubject(string monHoc)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.LocTheoMonHocAsync(monHoc, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // GET /api/v1/giao-an/loc-theo-khoi/{khoi} - Lọc theo khối lớp
        [HttpGet("loc-theo-khoi/{khoi:int}")]
        public async Task<IActionResult> FilterByGrade(int khoi)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.LocTheoKhoiAsync(khoi, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        // POST /api/v1/giao-an/{id}/xuat-pdf - Xuất PDF
        [HttpPost("{id:guid}/xuat-pdf")]
        public async Task<IActionResult> ExportPdf(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.XuatPDFAsync(id, teacherId);
            if (!rs.ThanhCong || rs.DuLieu == null) return BadRequest(rs);
            return File(rs.DuLieu, "application/pdf", $"giao-an-{id}.pdf");
        }

        // POST /api/v1/giao-an/{id}/xuat-word - Xuất Word
        [HttpPost("{id:guid}/xuat-word")]
        public async Task<IActionResult> ExportWord(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.XuatWordAsync(id, teacherId);
            if (!rs.ThanhCong || rs.DuLieu == null) return BadRequest(rs);
            return File(rs.DuLieu, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"giao-an-{id}.docx");
        }

        // POST /api/v1/giao-an/tu-mau/{templateId} - Tạo giáo án từ mẫu
        [HttpPost("tu-mau/{templateId:guid}")]
        public async Task<IActionResult> CreateFromTemplate(Guid templateId, [FromBody] YeuCauTaoGiaoAn request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<GiaoAn>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var teacherId = HttpContext.GetUserId();
            var rs = await _giaoAnService.TaoTuMauAsync(templateId, request, teacherId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }
    }
}