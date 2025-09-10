using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Services;

namespace PlanService.Controllers
{
    [ApiController]
    [Route("api/v1/chu-de")]
    public class ChuDeController : ControllerBase
    {
        private readonly IChuDeService _chuDeService;

        public ChuDeController(IChuDeService chuDeService)
        {
            _chuDeService = chuDeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rs = await _chuDeService.GetAllAsync();
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var rs = await _chuDeService.GetByIdAsync(id);
            return rs.ThanhCong ? Ok(rs) : NotFound(rs);
        }

        [HttpGet("cha-con")]
        public async Task<IActionResult> GetTree()
        {
            var rs = await _chuDeService.GetHierarchicalTreeAsync();
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [HttpGet("theo-mon/{monHoc}")]
        public async Task<IActionResult> GetBySubject(string monHoc)
        {
            var rs = await _chuDeService.GetByMonHocAsync(monHoc);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [HttpGet("parent/{parentId:guid?}")]
        public async Task<IActionResult> GetByParent(Guid? parentId)
        {
            var rs = await _chuDeService.GetByParentIdAsync(parentId);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChuDe request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<ChuDe>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            var rs = await _chuDeService.AddAsync(request);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChuDe request)
        {
            if (id == Guid.Empty)
                return BadRequest(ApiPhanHoi<ChuDe>.ThatBai("ID không hợp lệ"));
            if (!ModelState.IsValid)
                return BadRequest(ApiPhanHoi<ChuDe>.ThatBai("Dữ liệu không hợp lệ", ModelState));

            request.Id = id;
            var rs = await _chuDeService.UpdateAsync(request);
            return rs.ThanhCong ? Ok(rs) : BadRequest(rs);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var rs = await _chuDeService.DeleteAsync(id);
            return rs.ThanhCong ? Ok(rs) : NotFound(rs);
        }
    }
}