using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/tao-de-thi")]
    public class TaoDeThiController : ControllerBase
    {
        private readonly ITaoDeThiService _service;

        public TaoDeThiController(ITaoDeThiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo đề thi tự động theo ma trận kiến thức (số lượng câu hỏi cho từng chủ đề, độ khó).
        /// </summary>
        [HttpPost("tu-dong")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> CreateAutomatic([FromBody] TaoDeThiTuDongDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
            }
            
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CreateAutomaticAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return Created($"/api/v1/de-thi/{result.DuLieu.Id}", result);
        }

        /// <summary>
        /// Tạo đề thi mới từ một mẫu đề thi có sẵn.
        /// </summary>
        [HttpPost("tu-mau")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> CreateFromTemplate([FromBody] TaoDeThiTuMauDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
            }
            
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CreateFromTemplateAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return Created($"/api/v1/de-thi/{result.DuLieu.Id}", result);
        }

        /// <summary>
        /// Tạo đề thi bằng cách lấy ngẫu nhiên một số câu hỏi từ ngân hàng.
        /// </summary>
        [HttpPost("ngau-nhien")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> CreateRandom([FromBody] TaoDeThiNgauNhienDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
            }
            
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CreateRandomAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return Created($"/api/v1/de-thi/{result.DuLieu!.Id}", result);
        }

        /// <summary>
        /// Tạo đề thi từ một danh sách câu hỏi được chọn thủ công.
        /// </summary>
        [HttpPost("tu-ngan-hang")]
        [HttpPost("tu-cau-hoi")] // Alias để match với Postman
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> CreateFromBank([FromBody] TaoDeThiTuNganHangDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
            }
            
            var teacherId = HttpContext.GetUserId();
            var result = await _service.CreateFromBankAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return Created($"/api/v1/de-thi/{result.DuLieu!.Id}", result);
        }
    }
}