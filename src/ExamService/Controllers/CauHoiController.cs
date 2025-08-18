using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;
using TaskService.Repositories;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CauHoi>>> GetAll()
        {
            var cauHois = await _cauHoiRepository.GetAllAsync();
            return Ok(cauHois);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CauHoi>> GetById(string id)
        {
            var cauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (cauHoi == null)
            {
                return NotFound(new { Message = "Không tìm thấy câu hỏi" });
            }
            return Ok(cauHoi);
        }

        [HttpGet("mon-hoc/{monHoc}/do-kho/{doKho}")]
        public async Task<ActionResult<IEnumerable<CauHoi>>> GetByMonHocAndDoKho(string monHoc, string doKho)
        {
            var cauHois = await _cauHoiRepository.GetByMonHocAndDoKhoAsync(monHoc, doKho);
            return Ok(cauHois);
        }

        [HttpPost]
        public async Task<ActionResult<CauHoi>> Create([FromBody] CauHoi cauHoi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            cauHoi.Id = Guid.NewGuid();
            cauHoi.TaoLuc = DateTime.UtcNow;
            cauHoi.CapNhatLuc = DateTime.UtcNow;
            cauHoi.IsActive = true;

            var createdCauHoi = await _cauHoiRepository.CreateAsync(cauHoi);
            return CreatedAtAction(nameof(GetById), new { id = createdCauHoi.Id }, createdCauHoi);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CauHoi>> Update(string id, [FromBody] CauHoi cauHoi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (existingCauHoi == null)
            {
                return NotFound(new { Message = "Không tìm thấy câu hỏi" });
            }

            cauHoi.Id = existingCauHoi.Id;
            cauHoi.CapNhatLuc = DateTime.UtcNow;

            var updatedCauHoi = await _cauHoiRepository.UpdateAsync(cauHoi);
            return Ok(updatedCauHoi);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingCauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (existingCauHoi == null)
            {
                return NotFound(new { Message = "Không tìm thấy câu hỏi" });
            }

            await _cauHoiRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}