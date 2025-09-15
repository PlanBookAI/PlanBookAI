using Microsoft.AspNetCore.Mvc;
using ClassroomService.Models.DTOs;
using ClassroomService.Services.Interfaces;
using FluentValidation;

namespace ClassroomService.Controllers
{
    /// <summary>
    /// Controller for managing answer sheets
    /// </summary>
    [ApiController]
    [Route("api/v1/lop-hoc/answer-sheet")]
    public class AnswerSheetsController : ControllerBase
    {
        private readonly IAnswerSheetsService _answerSheetsService;
        private readonly IValidator<CreateAnswerSheetDto> _createValidator;
        private readonly ILogger<AnswerSheetsController> _logger;

        /// <summary>
        /// Initializes a new instance of AnswerSheetsController
        /// </summary>
        /// <param name="answerSheetsService">Answer sheets service</param>
        /// <param name="createValidator">Create answer sheet validator</param>
        /// <param name="logger">Logger instance</param>
        public AnswerSheetsController(
            IAnswerSheetsService answerSheetsService,
            IValidator<CreateAnswerSheetDto> createValidator,
            ILogger<AnswerSheetsController> logger)
        {
            _answerSheetsService = answerSheetsService;
            _createValidator = createValidator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new answer sheet
        /// </summary>
        /// <param name="dto">Answer sheet creation data</param>
        /// <returns>Created answer sheet</returns>
        [HttpPost]
        public async Task<ActionResult<object>> LuuBaiLam([FromBody] CreateAnswerSheetDto dto)
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
                
                var result = await _answerSheetsService.LuuBaiLam(dto);
                
                return CreatedAtAction(nameof(LayBaiLamTheoId), new { id = result.Id }, new
                {
                    success = true,
                    message = "Lưu bài làm thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu bài làm");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets answer sheets by student ID with pagination
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of answer sheets</returns>
        [HttpGet("hoc-sinh/{studentId}")]
        public async Task<ActionResult<object>> LayBaiLamTheoHocSinh(Guid studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount) = await _answerSheetsService.LayBaiLamTheoHocSinh(studentId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy bài làm theo học sinh thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy bài làm theo học sinh {StudentId}", studentId);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets answer sheets by exam ID with pagination
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of answer sheets</returns>
        [HttpGet("de-thi/{examId}")]
        public async Task<ActionResult<object>> LayBaiLamTheoDeThi(Guid examId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalCount) = await _answerSheetsService.LayBaiLamTheoDeThi(examId, page, pageSize);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy bài làm theo đề thi thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy bài làm theo đề thi {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Gets a specific answer sheet by ID
        /// </summary>
        /// <param name="id">Answer sheet ID</param>
        /// <returns>Answer sheet details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> LayBaiLamTheoId(Guid id)
        {
            try
            {
                var result = await _answerSheetsService.LayBaiLamTheoId(id);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bài làm" });
                }
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin bài làm thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin bài làm {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        /// <summary>
        /// Updates an existing answer sheet
        /// </summary>
        /// <param name="id">Answer sheet ID</param>
        /// <param name="dto">Answer sheet update data</param>
        /// <returns>Updated answer sheet</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> CapNhatBaiLam(Guid id, [FromBody] CreateAnswerSheetDto dto)
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
                
                var result = await _answerSheetsService.CapNhatBaiLam(id, dto);
                
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật bài làm thành công",
                    data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy bài làm" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bài làm {Id}", id);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }
    }
}
