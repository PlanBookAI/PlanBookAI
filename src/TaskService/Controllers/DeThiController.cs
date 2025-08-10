using Microsoft.AspNetCore.Mvc;
using TaskService.Models.Entities;
using TaskService.Models.DTOs;
using TaskService.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Added for logging
using System.Linq; // Added for Where and Select

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/v1/de-thi")]
    public class DeThiController : ControllerBase
    {
        private readonly IDeThiRepository _deThiRepository;
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ILogger<DeThiController> _logger; // Added logger field

        public DeThiController(IDeThiRepository deThiRepository, ICauHoiRepository cauHoiRepository, ILogger<DeThiController> logger) // Added logger to constructor
        {
            _deThiRepository = deThiRepository;
            _cauHoiRepository = cauHoiRepository;
            _logger = logger; // Initialize logger
        }

        /// <summary>
        /// Lấy danh sách tất cả đề thi
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeThi>>> LayDanhSachDeThi()
        {
            try
            {
                var danhSachDeThi = await _deThiRepository.GetAllAsync();
                return Ok(danhSachDeThi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách đề thi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy đề thi theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeThi>> LayDeThiTheoId(string id)
        {
            try
            {
                var deThi = await _deThiRepository.GetByIdAsync(id);
                if (deThi == null)
                {
                    return NotFound($"Không tìm thấy đề thi với ID: {id}");
                }
                return Ok(deThi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy đề thi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo đề thi mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DeThi>> TaoDeThi([FromBody] DeThi deThi)
        {
            try
            {
                if (deThi == null)
                {
                    return BadRequest("Dữ liệu đề thi không hợp lệ");
                }

                // Tạo ID mới
                deThi.id = Guid.NewGuid().ToString();
                
                var createdDeThi = await _deThiRepository.CreateAsync(deThi);
                return CreatedAtAction(nameof(LayDeThiTheoId), new { id = createdDeThi.id }, createdDeThi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo đề thi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo đề thi ngẫu nhiên từ ngân hàng câu hỏi
        /// </summary>
        [HttpPost("tao-ngau-nhien")]
        public async Task<ActionResult<DeThi>> TaoDeThiNgauNhien([FromBody] YeuCauTaoDeThi yeuCau)
        {
            try
            {
                // Log để debug
                _logger.LogInformation("TaoDeThiNgauNhien called with yeuCau: {@YeuCau}", yeuCau);
                _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();
                    
                    _logger.LogWarning("ModelState validation failed: {@Errors}", errors);
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }

                if (yeuCau == null)
                {
                    _logger.LogWarning("yeuCau is null");
                    return BadRequest("Yêu cầu tạo đề thi không hợp lệ");
                }

                _logger.LogInformation("Processing request: TieuDe={TieuDe}, MonHoc={MonHoc}, DoKho={DoKho}, SoLuongCauHoi={SoLuongCauHoi}, ThoiGianLam={ThoiGianLam}", 
                    yeuCau.TieuDe, yeuCau.MonHoc, yeuCau.DoKho, yeuCau.SoLuongCauHoi, yeuCau.ThoiGianLam);

                // Lấy câu hỏi theo môn học và độ khó
                var cauHoiTheoYeuCau = await _cauHoiRepository.GetByMonHocAndDoKhoAsync(yeuCau.MonHoc, yeuCau.DoKho);

                _logger.LogInformation("Found {Count} questions matching MonHoc={MonHoc}, DoKho={DoKho}", 
                    cauHoiTheoYeuCau.Count, yeuCau.MonHoc, yeuCau.DoKho);

                if (cauHoiTheoYeuCau.Count < yeuCau.SoLuongCauHoi)
                {
                    return BadRequest($"Không đủ câu hỏi. Cần {yeuCau.SoLuongCauHoi} nhưng chỉ có {cauHoiTheoYeuCau.Count}");
                }

                // Tạo đề thi mới
                var deThi = new DeThi
                {
                    id = Guid.NewGuid().ToString(),
                    tieuDe = yeuCau.TieuDe,
                    monHoc = yeuCau.MonHoc,
                    thoiGianLam = yeuCau.ThoiGianLam,
                    tongDiem = 10.0f
                };

                // Tạo ngẫu nhiên câu hỏi
                deThi.taoNgauNhien(cauHoiTheoYeuCau, yeuCau.SoLuongCauHoi);

                await _deThiRepository.CreateAsync(deThi);
                _logger.LogInformation("Successfully created DeThi with ID: {Id}", deThi.id);
                
                return CreatedAtAction(nameof(LayDeThiTheoId), new { id = deThi.id }, deThi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TaoDeThiNgauNhien");
                return StatusCode(500, $"Lỗi khi tạo đề thi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật đề thi
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhatDeThi(string id, [FromBody] DeThi deThi)
        {
            try
            {
                if (deThi == null || id != deThi.id)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                await _deThiRepository.UpdateAsync(deThi);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật đề thi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa đề thi
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaDeThi(string id)
        {
            try
            {
                await _deThiRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa đề thi: {ex.Message}");
            }
        }
    }
}
