using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/de-thi-cau-hoi")]
    public class DeThiCauHoiController : ControllerBase
    {
        private readonly IDeThiCauHoiService _service;

        public DeThiCauHoiController(IDeThiCauHoiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách các câu hỏi trong một đề thi.
        /// </summary>
        [HttpGet("de-thi/{deThiId:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<List<DeThiCauHoiResponseDTO>>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> GetByExamId(Guid deThiId)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetQuestionsByExamIdAsync(deThiId, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thứ tự của một câu hỏi trong đề thi.
        /// </summary>
        [HttpPut("{id:guid}/thu-tu")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiCauHoiResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] CapNhatThuTuDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.UpdateOrderAsync(id, dto.ThuTuMoi, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật điểm số của một câu hỏi trong đề thi.
        /// </summary>
        [HttpPut("{id:guid}/diem")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiCauHoiResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> UpdatePoints(Guid id, [FromBody] CapNhatDiemDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.UpdatePointsAsync(id, dto.DiemMoi, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }
    }
}