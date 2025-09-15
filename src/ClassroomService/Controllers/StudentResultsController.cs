using Microsoft.AspNetCore.Mvc;
using ClassroomService.Models.DTOs;
using ClassroomService.Services.Interfaces;
using FluentValidation;

namespace ClassroomService.Controllers
{
    /// <summary>
    /// Controller for managing student results
    /// </summary>
    [ApiController]
    [Route("api/v1/ket-qua-hoc-sinh")]
    public class StudentResultsController : ControllerBase
    {
        private readonly IStudentResultsService _studentResultsService;
        private readonly IValidator<CreateStudentResultDto> _createValidator;
        private readonly ILogger<StudentResultsController> _logger;

        /// <summary>
        /// Initializes a new instance of StudentResultsController
        /// </summary>
        /// <param name="studentResultsService">Student results service</param>
        /// <param name="createValidator">Create student result validator</param>
        /// <param name="logger">Logger instance</param>
        public StudentResultsController(
            IStudentResultsService studentResultsService,
            IValidator<CreateStudentResultDto> createValidator,
            ILogger<StudentResultsController> logger)
        {
            _studentResultsService = studentResultsService;
            _createValidator = createValidator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new student result
        /// </summary>
        /// <param name="dto">Student result creation data</param>
        /// <returns>Created student result</returns>
        [HttpPost]
        public async Task<ActionResult<object>> LuuKetQua([FromBody] CreateStudentResultDto dto)
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
                
                var result = await _studentResultsService.LuuKetQua(dto);
                
                return CreatedAtAction(nameof(LayKetQuaTheoId), new { id = result.Id }, new
                {
                    success = true,
                    message = "Lưu kết quả thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu kết quả học sinh");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets student results by student ID with pagination
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of student results</returns>
        [HttpGet("hoc-sinh/{studentId}")]
        public async Task<ActionResult<object>> LayKetQuaTheoHocSinh(Guid studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount) = await _studentResultsService.LayKetQuaTheoHocSinh(studentId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy kết quả theo học sinh thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy kết quả theo học sinh {StudentId}", studentId);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets student results by exam ID with pagination
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of student results</returns>
        [HttpGet("de-thi/{examId}")]
        public async Task<ActionResult<object>> LayKetQuaTheoDeThi(Guid examId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount) = await _studentResultsService.LayKetQuaTheoDeThi(examId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy kết quả theo đề thi thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy kết quả theo đề thi {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets a specific student result by ID
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <returns>Student result details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> LayKetQuaTheoId(Guid id)
        {
            try
            {
                var result = await _studentResultsService.LayKetQuaTheoId(id);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy kết quả" });
                }
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin kết quả thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin kết quả {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Updates an existing student result
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <param name="dto">Student result update data</param>
        /// <returns>Updated student result</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> CapNhatKetQua(Guid id, [FromBody] CreateStudentResultDto dto)
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
                
                var result = await _studentResultsService.CapNhatKetQua(id, dto);
                
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật kết quả thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy kết quả" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật kết quả {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }
    }
}
