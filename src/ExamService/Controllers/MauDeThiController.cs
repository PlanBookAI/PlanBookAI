using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/mau-de-thi")]
    public class MauDeThiController : ControllerBase
    {
        private readonly IMauDeThiService _service;

        public MauDeThiController(IMauDeThiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo một mẫu đề thi mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiPhanHoi<MauDeThiResponseDTO>), 201)]
        public async Task<IActionResult> Create([FromBody] MauDeThiRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CreateAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.DuLieu!.Id }, result);
        }

        /// <summary>
        /// Lấy danh sách các mẫu đề thi của giáo viên (có phân trang).
        /// </summary>
        /// <param name="paging">Tham số phân trang.</param>
        /// <returns>Danh sách các mẫu đề thi.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<MauDeThiResponseDTO>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] PagingDTO paging)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetAllAsync(teacherId, paging);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một mẫu đề thi theo ID.
        /// </summary>
        /// <param name="id">ID của mẫu đề thi.</param>
        /// <returns>Thông tin chi tiết của mẫu đề thi.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<MauDeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetByIdAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin của một mẫu đề thi đã có.
        /// </summary>
        /// <param name="id">ID của mẫu đề thi cần cập nhật.</param>
        /// <param name="dto">Dữ liệu mới cho mẫu đề thi.</param>
        /// <returns>Thông tin mẫu đề thi sau khi đã được cập nhật.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<MauDeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauDeThiRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.UpdateAsync(id, dto, teacherId);

            if (result.MaTrangThai != 200)
            {
                return NotFound(result); // Nếu không tìm thấy hoặc không có quyền
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa một mẫu đề thi.
        /// </summary>
        /// <param name="id">ID của mẫu đề thi cần xóa.</param>
        /// <returns>Trạng thái thành công của thao tác xóa.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.DeleteAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                return NotFound(result); // Trả về 404 nếu không tìm thấy hoặc không có quyền
            }

            return Ok(result); // Trả về 200 OK với kết quả { success: true, ... }
        }

        /// <summary>
        /// Tạo một bản sao từ một mẫu đề thi đã có.
        /// </summary>
        /// <param name="id">ID của mẫu đề thi gốc cần sao chép.</param>
        /// <returns>Thông tin chi tiết của mẫu đề thi mới vừa được tạo.</returns>
        [HttpPost("{id:guid}/sao-chep")]
        [ProducesResponseType(typeof(ApiPhanHoi<MauDeThiResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Clone(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CloneAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                return NotFound(result);
            }

            // Trả về 201 Created cùng với thông tin của tài nguyên mới
            return CreatedAtAction(nameof(GetById), new { id = result.DuLieu!.Id }, result);
        }
    }
}