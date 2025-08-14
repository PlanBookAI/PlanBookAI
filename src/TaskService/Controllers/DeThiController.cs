using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.DTOs;
using TaskService.Models.Entities;
using TaskService.Repositories;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/v1/de-thi")]
    public class DeThiController : ControllerBase
    {
        private readonly IDeThiRepository _deThiRepository;

        public DeThiController(IDeThiRepository deThiRepository)
        {
            _deThiRepository = deThiRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeThi>>> GetAll()
        {
            var deThis = await _deThiRepository.GetAllAsync();
            return Ok(deThis);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeThi>> GetById(string id)
        {
            var deThi = await _deThiRepository.GetByIdAsync(id);
            if (deThi == null)
            {
                return NotFound(new { Message = "Không tìm thấy đề thi" });
            }
            return Ok(deThi);
        }

        [HttpPost]
        public async Task<ActionResult<DeThi>> Create([FromBody] YeuCauTaoDeThi yeuCau)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deThi = new DeThi
            {
                Id = Guid.NewGuid(),
                TieuDe = yeuCau.TieuDe,
                MonHoc = yeuCau.MonHoc,
                KhoiLop = yeuCau.KhoiLop,
                ThoiGianLamBai = yeuCau.ThoiGianLamBai,
                HuongDan = yeuCau.HuongDan,
                TrangThai = "draft",
                NguoiTaoId = Guid.Parse(User.Identity?.Name ?? throw new InvalidOperationException("User not authenticated")),
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            };

            var createdDeThi = await _deThiRepository.CreateAsync(deThi);
            return CreatedAtAction(nameof(GetById), new { id = createdDeThi.Id }, createdDeThi);
        }

        [HttpPost("{id}/nop-bai")]
        public async Task<ActionResult<PhanHoiDeThi>> NopBai(string id, [FromBody] YeuCauNopBai yeuCau)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deThi = await _deThiRepository.GetByIdAsync(id);
            if (deThi == null)
            {
                return NotFound(new { Message = "Không tìm thấy đề thi" });
            }

            // TODO: Implement nộp bài logic
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DeThi>> Update(string id, [FromBody] DeThi deThi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDeThi = await _deThiRepository.GetByIdAsync(id);
            if (existingDeThi == null)
            {
                return NotFound(new { Message = "Không tìm thấy đề thi" });
            }

            deThi.Id = existingDeThi.Id;
            deThi.CapNhatLuc = DateTime.UtcNow;

            var updatedDeThi = await _deThiRepository.UpdateAsync(deThi);
            return Ok(updatedDeThi);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingDeThi = await _deThiRepository.GetByIdAsync(id);
            if (existingDeThi == null)
            {
                return NotFound(new { Message = "Không tìm thấy đề thi" });
            }

            await _deThiRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}