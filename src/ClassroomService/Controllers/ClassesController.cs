using Microsoft.AspNetCore.Mvc;
using ClassroomService.Models.DTOs;
using ClassroomService.Services.Interfaces;
using FluentValidation;

namespace ClassroomService.Controllers
{
    [ApiController]
    [Route("api/v1/lop-hoc")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassesService _classesService;
        private readonly IValidator<CreateClassDto> _createValidator;
        private readonly IValidator<UpdateClassDto> _updateValidator;
        private readonly ILogger<ClassesController> _logger;

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

        [HttpGet]
        public async Task<ActionResult<object>> LayDanhSachLopHoc([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                int? homeroomTeacherId = userRole == "TEACHER" ? userId : null;
                
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

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> LayLopHocTheoId(int id)
        {
            try
            {
                var userId = GetUserIdFromHeader();
                var userRole = GetUserRoleFromHeader();
                
                int? homeroomTeacherId = userRole == "TEACHER" ? userId : null;
                
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

        private int GetUserIdFromHeader()
        {
            var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            return int.TryParse(userIdHeader, out int userId) ? userId : 0;
        }

        private string GetUserRoleFromHeader()
        {
            return Request.Headers["X-User-Role"].FirstOrDefault() ?? "TEACHER";
        }

        private string GetUserEmailFromHeader()
        {
            return Request.Headers["X-User-Email"].FirstOrDefault();
        }
    }
}