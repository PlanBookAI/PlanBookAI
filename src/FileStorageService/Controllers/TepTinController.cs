namespace PlanbookAI.FileStorageService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using PlanbookAI.FileStorageService.Models.DTOs;
    using PlanbookAI.FileStorageService.Services;
    
    [ApiController]
    [Route("api/v1/tep-tin")]
    public class TepTinController : ControllerBase
    {
        private readonly IDichVuTepTin _dichVuTepTin;
        private readonly ILogger<TepTinController> _logger;
        
        public TepTinController(IDichVuTepTin dichVuTepTin, ILogger<TepTinController> logger)
        {
            _dichVuTepTin = dichVuTepTin;
            _logger = logger;
        }
        
        private Guid GetUserId()
        {
            var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            return Guid.TryParse(userIdHeader, out var userId) ? userId : Guid.Empty;
        }
        
        private bool IsAdmin()
        {
            var roleHeader = Request.Headers["X-User-Role"].FirstOrDefault();
            return "ADMIN".Equals(roleHeader, StringComparison.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Tải lên tệp tin mới
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhanHoiTepTin>> Upload([FromForm] YeuCauUploadTepTin request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(new PhanHoiLoi { ThongBao = "Thiếu thông tin người dùng" });
                }
                
                if (request.Tep == null || request.Tep.Length == 0)
                {
                    return BadRequest(new PhanHoiLoi { ThongBao = "Không có tệp tin được tải lên" });
                }
                
                var result = await _dichVuTepTin.UploadAsync(request, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new PhanHoiLoi { ThongBao = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải lên tệp tin");
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Lấy thông tin chi tiết tệp tin
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PhanHoiTepTin>> GetById(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var file = await _dichVuTepTin.GetByIdAsync(id, userId, isAdmin);
                if (file == null)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin" });
                }
                
                var response = new PhanHoiTepTin
                {
                    Id = file.Id,
                    TenGoc = file.OriginalName,
                    KichThuoc = file.FileSize,
                    MimeType = file.MimeType,
                    Loai = file.FileType,
                    TrangThai = file.Status,
                    NgayTao = file.CreatedAt,
                    ThuocTinh = file.Metadata?.ToDictionary(m => m.Key, m => m.Value)
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Tải xuống tệp tin
        /// </summary>
        [HttpGet("{id}/tai-xuong")]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var file = await _dichVuTepTin.GetByIdAsync(id, userId, isAdmin);
                if (file == null)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin" });
                }
                
                var storageRoot = Environment.GetEnvironmentVariable("FILE_STORAGE_ROOT") ?? "./data/files";
                var fullPath = Path.Combine(storageRoot, file.FilePath);
                
                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Tệp tin vật lý không tồn tại" });
                }
                
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                var contentDisposition = $"attachment; filename=\"{file.OriginalName}\"";
                
                Response.Headers.Add("Content-Disposition", contentDisposition);
                
                return File(fileStream, file.MimeType, file.OriginalName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải xuống tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Lấy danh sách tệp tin với phân trang
        /// </summary>
        [HttpGet("danh-sach")]
        public async Task<ActionResult<PhanHoiDanhSachTepTin>> GetList(
            [FromQuery(Name = "trang")] int trang = 1,
            [FromQuery(Name = "kich-thuoc")] int kichThuoc = 20,
            [FromQuery(Name = "loai")] string? loai = null,
            [FromQuery(Name = "trang-thai")] string? trangThai = null)
        {
            try
            {
                if (trang < 1) trang = 1;
                if (kichThuoc < 1 || kichThuoc > 100) kichThuoc = 20;
                
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var result = await _dichVuTepTin.GetPagedAsync(trang, kichThuoc, loai, trangThai, userId, isAdmin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tệp tin");
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Xóa mềm tệp tin (chuyển trạng thái DELETED)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var success = await _dichVuTepTin.SoftDeleteAsync(id, userId, isAdmin);
                if (!success)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin hoặc không có quyền" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mềm tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Khôi phục tệp tin từ trạng thái DELETED
        /// </summary>
        [HttpPost("{id}/khoi-phuc")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var success = await _dichVuTepTin.RestoreAsync(id, userId, isAdmin);
                if (!success)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin hoặc không có quyền" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khôi phục tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Xóa vĩnh viễn tệp tin (chỉ ADMIN)
        /// </summary>
        [HttpDelete("{id}/xoa-vinh-vien")]
        public async Task<IActionResult> PermanentDelete(Guid id)
        {
            try
            {
                var isAdmin = IsAdmin();
                if (!isAdmin)
                {
                    return Forbid();
                }
                
                var success = await _dichVuTepTin.PermanentDeleteAsync(id, isAdmin);
                if (!success)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa vĩnh viễn tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Cập nhật metadata của tệp tin
        /// </summary>
        [HttpPut("{id}/metadata")]
        public async Task<IActionResult> UpdateMetadata(Guid id, [FromBody] YeuCauCapNhatMetadata request)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                if (request.ThuocTinh == null || !request.ThuocTinh.Any())
                {
                    return BadRequest(new PhanHoiLoi { ThongBao = "Dữ liệu metadata không hợp lệ" });
                }
                
                var success = await _dichVuTepTin.UpdateMetadataAsync(id, request.ThuocTinh, userId, isAdmin);
                if (!success)
                {
                    return NotFound(new PhanHoiLoi { ThongBao = "Không tìm thấy tệp tin hoặc không có quyền" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật metadata tệp tin {FileId}", id);
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Tìm kiếm tệp tin theo metadata
        /// </summary>
        [HttpGet("tim-kiem")]
        public async Task<ActionResult<List<PhanHoiTepTin>>> Search(
            [FromQuery] string key,
            [FromQuery] string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                {
                    return BadRequest(new PhanHoiLoi { ThongBao = "Key và value là bắt buộc" });
                }
                
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                
                var results = await _dichVuTepTin.SearchByMetadataAsync(key, value, userId, isAdmin);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm tệp tin theo metadata");
                return StatusCode(500, new PhanHoiLoi { ThongBao = "Lỗi hệ thống" });
            }
        }
        
        /// <summary>
        /// Kiểm tra sức khỏe của dịch vụ
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                // Kiểm tra quyền ghi thư mục
                var storageRoot = Environment.GetEnvironmentVariable("FILE_STORAGE_ROOT") ?? "./data/files";
                var testFile = Path.Combine(storageRoot, "health_test.tmp");
                
                Directory.CreateDirectory(storageRoot);
                await System.IO.File.WriteAllTextAsync(testFile, "health check");
                System.IO.File.Delete(testFile);
                
                return Ok(new { trangThai = "OK", thoiGian = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new { trangThai = "ERROR", thongBao = "Không thể ghi vào thư mục lưu trữ" });
            }
        }
    }
}