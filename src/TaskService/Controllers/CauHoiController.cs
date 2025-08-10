using Microsoft.AspNetCore.Mvc;
using TaskService.Models.Entities;
using TaskService.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/v1/cau-hoi")]
    public class CauHoiController : ControllerBase
    {
        private readonly ICauHoiRepository _cauHoiRepository;

        public CauHoiController(ICauHoiRepository cauHoiRepository)
        {
            _cauHoiRepository = cauHoiRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả câu hỏi
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CauHoi>>> LayDanhSachCauHoi()
        {
            try
            {
                var danhSachCauHoi = await _cauHoiRepository.GetAllAsync();
                return Ok(danhSachCauHoi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách câu hỏi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CauHoi>> LayCauHoiTheoId(string id)
        {
            try
            {
                var cauHoi = await _cauHoiRepository.GetByIdAsync(Guid.Parse(id));
                if (cauHoi == null)
                {
                    return NotFound($"Không tìm thấy câu hỏi với ID: {id}");
                }
                return Ok(cauHoi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy câu hỏi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo môn học
        /// </summary>
        [HttpGet("mon-hoc/{monHoc}")]
        public async Task<ActionResult<IEnumerable<CauHoi>>> LayCauHoiTheoMonHoc(string monHoc)
        {
            try
            {
                var tatCaCauHoi = await _cauHoiRepository.GetAllAsync();
                var cauHoiTheoMonHoc = new List<CauHoi>();

                foreach (var cauHoi in tatCaCauHoi)
                {
                    if (cauHoi.monHoc.Equals(monHoc, StringComparison.OrdinalIgnoreCase))
                    {
                        cauHoiTheoMonHoc.Add(cauHoi);
                    }
                }

                return Ok(cauHoiTheoMonHoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy câu hỏi theo môn học: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo độ khó
        /// </summary>
        [HttpGet("do-kho/{doKho}")]
        public async Task<ActionResult<IEnumerable<CauHoi>>> LayCauHoiTheoDoKho(string doKho)
        {
            try
            {
                var tatCaCauHoi = await _cauHoiRepository.GetAllAsync();
                var cauHoiTheoDoKho = new List<CauHoi>();

                foreach (var cauHoi in tatCaCauHoi)
                {
                    if (cauHoi.doKho.Equals(doKho, StringComparison.OrdinalIgnoreCase))
                    {
                        cauHoiTheoDoKho.Add(cauHoi);
                    }
                }

                return Ok(cauHoiTheoDoKho);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy câu hỏi theo độ khó: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo câu hỏi mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CauHoi>> TaoCauHoi([FromBody] CauHoi cauHoi)
        {
            try
            {
                if (cauHoi == null)
                {
                    return BadRequest("Dữ liệu câu hỏi không hợp lệ");
                }

                // Tạo ID mới
                cauHoi.id = Guid.NewGuid().ToString();
                
                await _cauHoiRepository.AddAsync(cauHoi);
                return CreatedAtAction(nameof(LayCauHoiTheoId), new { id = cauHoi.id }, cauHoi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo câu hỏi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật câu hỏi
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhatCauHoi(string id, [FromBody] CauHoi cauHoi)
        {
            try
            {
                if (cauHoi == null || id != cauHoi.id)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                await _cauHoiRepository.UpdateAsync(cauHoi);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật câu hỏi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa câu hỏi
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaCauHoi(string id)
        {
            try
            {
                await _cauHoiRepository.DeleteAsync(Guid.Parse(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa câu hỏi: {ex.Message}");
            }
        }
    }
}
