using Microsoft.AspNetCore.Mvc;
using ClassroomService.Models.DTOs;
using ClassroomService.Services.Interfaces;
using FluentValidation;

namespace ClassroomService.Controllers
{
    /// <summary>
    /// Controller for managing students
    /// </summary>
    [ApiController]
    [Route("api/v1/hoc-sinh")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsService _studentsService;
        private readonly IValidator<CreateStudentDto> _createValidator;
        private readonly IValidator<UpdateStudentDto> _updateValidator;
        private readonly ILogger<StudentsController> _logger;

        /// <summary>
        /// Initializes a new instance of StudentsController
        /// </summary>
        /// <param name="studentsService">Students service</param>
        /// <param name="createValidator">Create student validator</param>
        /// <param name="updateValidator">Update student validator</param>
        /// <param name="logger">Logger instance</param>
        public StudentsController(
            IStudentsService studentsService,
            IValidator<CreateStudentDto> createValidator,
            IValidator<UpdateStudentDto> updateValidator,
            ILogger<StudentsController> logger)
        {
            _studentsService = studentsService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Gets paginated list of students
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of students</returns>
        [HttpGet]
        public async Task<ActionResult<object>> LayDanhSachHocSinh([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                Guid? ownerTeacherId = userRole == "TEACHER" ? userId : null;
                
                var (items, totalCount) = await _studentsService.LayDanhSachHocSinh(ownerTeacherId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách học sinh thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách học sinh");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets a specific student by ID
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>Student details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> LayHocSinhTheoId(Guid id)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                Guid? ownerTeacherId = userRole == "TEACHER" ? userId : null;
                
                var result = await _studentsService.LayHocSinhTheoId(id, ownerTeacherId);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin học sinh thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy học sinh" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin học sinh {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets students by class ID with pagination
        /// </summary>
        /// <param name="classId">Class ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of students in class</returns>
        [HttpGet("lop/{classId}")]
        public async Task<ActionResult<object>> LayHocSinhTheoLop(Guid classId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                Guid? ownerTeacherId = userRole == "TEACHER" ? userId : null;
                
                var (items, totalCount) = await _studentsService.LayHocSinhTheoLop(classId, ownerTeacherId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách học sinh theo lớp thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách học sinh theo lớp {ClassId}", classId);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Creates a new student
        /// </summary>
        /// <param name="dto">Student creation data</param>
        /// <returns>Created student</returns>
        [HttpPost]
        public async Task<ActionResult<object>> ThemHocSinh([FromBody] CreateStudentDto dto)
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
                dto.OwnerTeacherId = userId;
                
                var result = await _studentsService.ThemHocSinh(dto);
                
                return CreatedAtAction(nameof(LayHocSinhTheoId), new { id = result.Id }, new
                {
                    success = true,
                    message = "Thêm học sinh thành công",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm học sinh");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Updates an existing student
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="dto">Student update data</param>
        /// <returns>Updated student</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> CapNhatHocSinh(Guid id, [FromBody] UpdateStudentDto dto)
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
                var result = await _studentsService.CapNhatHocSinh(id, dto, userId);
                
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật học sinh thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy học sinh" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật học sinh {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Deletes a student by ID
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>Success result</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> XoaHocSinh(Guid id)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var result = await _studentsService.XoaHocSinh(id, userId);
                
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy học sinh" });
                }
                
                return Ok(new
                {
                    success = true,
                    message = "Xóa học sinh thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa học sinh {Id}", id);
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
    }
}