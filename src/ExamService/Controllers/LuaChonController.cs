using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/lua-chon")]
    public class LuaChonController : ControllerBase
    {
        private readonly ILuaChonService _luaChonService;

        public LuaChonController(ILuaChonService luaChonService)
        {
            _luaChonService = luaChonService;
        }

        /// <summary>
        /// Lấy tất cả các lựa chọn của một câu hỏi cụ thể.
        /// </summary>
        [HttpGet("cau-hoi/{cauHoiId:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<List<LuaChonResponseDTO>>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> GetByQuestionId(Guid cauHoiId)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _luaChonService.GetChoicesByQuestionIdAsync(cauHoiId, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Tạo một lựa chọn mới cho một câu hỏi.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiPhanHoi<LuaChonResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> Create([FromBody] TaoLuaChonRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _luaChonService.CreateAsync(dto, teacherId);
            if (!result.ThanhCong) return BadRequest(result);
            // Không trả về CreatedAtAction vì không có endpoint GetById cho lựa chọn
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật nội dung hoặc trạng thái của một lựa chọn.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<LuaChonResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] LuaChonRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _luaChonService.UpdateAsync(id, dto, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Xóa một lựa chọn khỏi câu hỏi.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<bool>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _luaChonService.DeleteAsync(id, teacherId);
            if (!result.ThanhCong) return NotFound(result);
            return Ok(result);
        }
    }
}