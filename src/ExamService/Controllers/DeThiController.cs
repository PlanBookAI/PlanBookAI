using ExamService.Models.DTOs;
using ExamService.Extensions;
using ExamService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/de-thi")]
    public class DeThiController : ControllerBase
    {
        private readonly IDeThiService _deThiService;

        public DeThiController(IDeThiService deThiService)
        {
            _deThiService = deThiService;
        }

        /// <summary>
        /// Lấy danh sách đề thi của giáo viên (có phân trang).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<DeThiResponseDTO>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.GetAllAsync(teacherId, pagingParams);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một đề thi.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.GetByIdAsync(id, teacherId);
            if (result.MaTrangThai != 200) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Tạo một đề thi mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 201)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 400)]
        public async Task<IActionResult> Create([FromBody] DeThiRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiPhanHoi<object>.ThatBai("Dữ liệu đầu vào không hợp lệ", 400));
            }
            
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.CreateAsync(dto, teacherId);
            if (result.MaTrangThai != 200) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.DuLieu!.Id }, result);
        }

        /// <summary>
        /// Cập nhật thông tin của một đề thi.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] DeThiRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.UpdateAsync(id, dto, teacherId);
            if (result.MaTrangThai != 200) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Xóa một đề thi.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<bool>), 200)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), 404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.DeleteAsync(id, teacherId);
            if (result.MaTrangThai != 200) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Thêm một câu hỏi từ ngân hàng vào một đề thi cụ thể.
        /// </summary>
        /// <param name="id">ID của đề thi.</param>
        /// <param name="dto">Thông tin câu hỏi cần thêm (ID và điểm số).</param>
        /// <returns>Thông tin đề thi sau khi đã được cập nhật.</returns>
        [HttpPost("{id:guid}/them-cau-hoi")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddQuestionToExam(Guid id, [FromBody] ThemCauHoiVaoDeThiDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.AddQuestionToExamAsync(id, dto, teacherId);

            if (result.MaTrangThai != 200)
            {
                // Phân biệt lỗi do không tìm thấy hay do logic nghiệp vụ
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa một câu hỏi ra khỏi một đề thi cụ thể.
        /// </summary>
        /// <param name="id">ID của đề thi.</param>
        /// <param name="cauHoiId">ID của câu hỏi cần xóa.</param>
        /// <returns>Thông tin đề thi sau khi đã được cập nhật.</returns>
        [HttpDelete("{id:guid}/cau-hoi/{cauHoiId:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveQuestionFromExam(Guid id, Guid cauHoiId)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.RemoveQuestionFromExamAsync(id, cauHoiId, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin của một câu hỏi trong đề thi (ví dụ: thay đổi điểm).
        /// </summary>
        /// <param name="id">ID của đề thi.</param>
        /// <param name="cauHoiId">ID của câu hỏi cần cập nhật.</param>
        /// <param name="dto">Dữ liệu cần cập nhật.</param>
        /// <returns>Thông tin đề thi sau khi đã được cập nhật.</returns>
        [HttpPut("{id:guid}/cau-hoi/{cauHoiId:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuestionInExam(Guid id, Guid cauHoiId, [FromBody] CapNhatCauHoiTrongDeThiDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.UpdateQuestionInExamAsync(id, cauHoiId, dto, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xuất bản một đề thi, chuyển trạng thái từ "draft" sang "published".
        /// Sau khi xuất bản, đề thi sẽ không thể chỉnh sửa nội dung được nữa.
        /// </summary>
        /// <param name="id">ID của đề thi cần xuất bản.</param>
        /// <returns>Thông tin đề thi sau khi đã được cập nhật trạng thái.</returns>
        [HttpPost("{id:guid}/xuat-ban")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishExam(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.PublishExamAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Hủy xuất bản một đề thi, chuyển trạng thái từ "published" về "draft".
        /// Thao tác này chỉ thành công khi chưa có học sinh nào làm bài thi này.
        /// </summary>
        /// <param name="id">ID của đề thi cần hủy xuất bản.</param>
        /// <returns>Thông tin đề thi sau khi đã được cập nhật trạng thái.</returns>
        [HttpPost("{id:guid}/huy-xuat-ban")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnpublishExam(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.UnpublishExamAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xuất một đề thi ra file PDF.
        /// </summary>
        /// <param name="id">ID của đề thi cần xuất file.</param>
        /// <returns>Một file .pdf chứa nội dung đề thi.</returns>
        [HttpGet("{id:guid}/xuat-pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportToPdf(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.ExportToPdfAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            // Tạo tên file từ tiêu đề đề thi
            // (Cần một hàm để xóa các ký tự không hợp lệ trong tên file)
            var deThi = await _deThiService.GetByIdAsync(id, teacherId);
            string fileName = $"{deThi.DuLieu?.TieuDe ?? "DeThi"}.pdf";

            return File(result.DuLieu, "application/pdf", fileName);
        }

        /// <summary>
        /// Xuất một đề thi ra file Word (.docx).
        /// </summary>
        /// <param name="id">ID của đề thi cần xuất file.</param>
        /// <returns>Một file .docx chứa nội dung đề thi.</returns>
        [HttpGet("{id:guid}/xuat-word")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportToWord(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.ExportToWordAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            var deThi = await _deThiService.GetByIdAsync(id, teacherId);
            string fileName = $"{deThi.DuLieu?.TieuDe ?? "DeThi"}.docx";

            // Kiểu MIME cho file Word
            string mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return File(result.DuLieu, mimeType, fileName);
        }

        /// <summary>
        /// Tạo một bản sao từ một đề thi đã có.
        /// Bản sao mới sẽ ở trạng thái "draft" và có tiêu đề được thêm hậu tố "- Bản sao".
        /// </summary>
        /// <param name="id">ID của đề thi gốc cần sao chép.</param>
        /// <returns>Thông tin chi tiết của đề thi mới vừa được tạo.</returns>
        [HttpPost("{id:guid}/sao-chep")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloneExam(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.CloneExamAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                return NotFound(result);
            }

            // Trả về 201 Created cùng với thông tin của tài nguyên mới
            return CreatedAtAction(nameof(GetById), new { id = result.DuLieu!.Id }, result);
        }

        /// <summary>
        /// Lấy thông tin thống kê chi tiết về kết quả của một đề thi.
        /// </summary>
        /// <param name="id">ID của đề thi cần xem thống kê.</param>
        /// <returns>Báo cáo thống kê chi tiết.</returns>
        [HttpGet("{id:guid}/thong-ke")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiThongKeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExamStatistics(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.GetExamStatisticsAsync(id, teacherId);

            if (result.MaTrangThai != 200)
            {
                // Trả về NotFound nếu không tìm thấy, BadRequest cho các lỗi khác (vd: chưa ai nộp bài)
                if (result.ThongBao.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm đề thi theo từ khóa
        /// </summary>
        [HttpGet("tim-kiem")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<DeThiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchExams([FromQuery] string keyword, [FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.SearchExamsAsync(teacherId, keyword, pagingParams);
            return Ok(result);
        }

        /// <summary>
        /// Lọc đề thi theo môn học
        /// </summary>
        [HttpGet("loc-mon-hoc")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<DeThiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterBySubject([FromQuery] string subject, [FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.FilterBySubjectAsync(teacherId, subject, pagingParams);
            return Ok(result);
        }

        /// <summary>
        /// Lọc đề thi theo khối lớp
        /// </summary>
        [HttpGet("loc-khoi-lop")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<DeThiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByGrade([FromQuery] int grade, [FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.FilterByGradeAsync(teacherId, grade, pagingParams);
            return Ok(result);
        }

        /// <summary>
        /// Lọc đề thi theo trạng thái
        /// </summary>
        [HttpGet("loc-trang-thai")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<DeThiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByStatus([FromQuery] string status, [FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _deThiService.FilterByStatusAsync(teacherId, status, pagingParams);
            return Ok(result);
        }


    }
}