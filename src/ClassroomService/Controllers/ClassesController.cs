using Microsoft.AspNetCore.Mvc;
using ClassroomService.Models.DTOs;
using ClassroomService.Services.Interfaces;
using FluentValidation;

namespace ClassroomService.Controllers
{
    /// <summary>
    /// Controller for managing school classes
    /// </summary>
    [ApiController]
    [Route("api/v1/lop-hoc")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassesService _classesService;
        private readonly IValidator<CreateClassDto> _createValidator;
        private readonly IValidator<UpdateClassDto> _updateValidator;
        private readonly ILogger<ClassesController> _logger;

        /// <summary>
        /// Initializes a new instance of ClassesController
        /// </summary>
        /// <param name="classesService">Classes service</param>
        /// <param name="createValidator">Create class validator</param>
        /// <param name="updateValidator">Update class validator</param>
        /// <param name="logger">Logger instance</param>
        public ClassesController(
            IClassesService classesService, 
            IValidator<CreateClassDto> createValidator,
            IValidator<UpdateClassDto> updateValidator,
            ILogger<ClassesController> logger)
        {
            _classesService = classesService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Gets paginated list of classes
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of classes</returns>
        [HttpGet]
        public async Task<ActionResult<object>> LayDanhSachLopHoc([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                Guid? homeroomTeacherId = userRole == "TEACHER" ? userId : null;
                
                var (items, totalCount) = await _classesService.LayDanhSachLopHoc(homeroomTeacherId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách lớp học thành công",
                    data = new
                    {
                        items,
                        totalCount,
                        page,
                        pageSize,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lớp học");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets a specific class by ID
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <returns>Class details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> LayLopHocTheoId(Guid id)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                Guid? homeroomTeacherId = userRole == "TEACHER" ? userId : null;
                
                var result = await _classesService.LayLopHocTheoId(id, homeroomTeacherId);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin lớp học thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lớp học" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin lớp học {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="dto">Class creation data</param>
        /// <returns>Created class</returns>
        [HttpPost]
        public async Task<ActionResult<object>> TaoLopHoc([FromBody] CreateClassDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }
                
                var userId = GetUserIdFromHeader();
                var result = await _classesService.TaoLopHoc(dto, userId);
                
                return CreatedAtAction(nameof(LayLopHocTheoId), new { id = result.Id }, new
                {
                    success = true,
                    message = "Tạo lớp học thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lớp học");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Updates an existing class
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <param name="dto">Class update data</param>
        /// <returns>Updated class</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> CapNhatLopHoc(Guid id, [FromBody] UpdateClassDto dto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }
                
                var userId = GetUserIdFromHeader();
                var result = await _classesService.CapNhatLopHoc(id, dto, userId);
                
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật lớp học thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lớp học" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lớp học {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Deletes a class
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> XoaLopHoc(Guid id)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var result = await _classesService.XoaLopHoc(id, userId);
                
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa lớp học thành công"
                    });
                }
                else
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lớp học" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lớp học {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        private Guid GetUserIdFromHeader()
        {
            var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            return Guid.TryParse(userIdHeader, out Guid userId) ? userId : Guid.Empty;
        }

        private string GetUserRoleFromHeader()
        {
            return Request.Headers["X-User-Role"].FirstOrDefault() ?? "TEACHER";
        }

        private string? GetUserEmailFromHeader()
        {
            return Request.Headers["X-User-Email"].FirstOrDefault();
        }
    }
}