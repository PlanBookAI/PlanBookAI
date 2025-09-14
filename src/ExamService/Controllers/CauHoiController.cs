using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ExamService.Models.Entities;
using ExamService.Repositories;
using ExamService.Models.DTOs;
using ExamService.Interfaces;
using ExamService.Extensions;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/cau-hoi")]
    public class CauHoiController : ControllerBase
    {
        private readonly ICauHoiService _cauHoiService;
        public CauHoiController(ICauHoiService cauHoiService)
        {
            _cauHoiService = cauHoiService;
        }

        /// <summary>
        /// Lấy danh sách câu hỏi của giáo viên (hỗ trợ phân trang, lọc và sắp xếp).
        /// </summary>
        /// <param name="pagingParams">Tham số phân trang.</param>
        /// <returns>Danh sách câu hỏi.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.GetAllAsync(teacherId, pagingParams);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một câu hỏi.
        /// </summary>
        /// <param name="id">ID của câu hỏi</param>
        /// <returns>Thông tin chi tiết câu hỏi.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<CauHoiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.GetByIdAsync(id, teacherId);
            if (result.MaTrangThai != 200)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Tạo một câu hỏi mới.
        /// </summary>
        /// <param name="dto">Dữ liệu để tạo câu hỏi.</param>
        /// <returns>Câu hỏi vừa được tạo.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiPhanHoi<CauHoiResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CauHoiRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.CreateAsync(dto, teacherId);
            if (result.MaTrangThai != 200)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { id = result.DuLieu.Id }, result);
        }

        /// <summary>
        /// Cập nhật thông tin một câu hỏi.
        /// </summary>
        /// <param name="id">ID của câu hỏi cần cập nhật.</param>
        /// <param name="dto">Dữ liệu mới của câu hỏi.</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<CauHoiResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CauHoiRequestDTO dto)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.UpdateAsync(id, dto, teacherId);
            if (result.MaTrangThai != 200)
            {
                return NotFound(result); // Or BadRequest depending on the error
            }
            return Ok(result);
        }

        /// <summary>
        /// Xóa một câu hỏi.
        /// </summary>
        /// <param name="id">ID của câu hỏi cần xóa.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiPhanHoi<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.DeleteAsync(id, teacherId);
            if (result.MaTrangThai != 200)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // Các endpoints còn lại sẽ được triển khai dần
        /// <summary>
        /// Tìm kiếm, lọc và sắp xếp câu hỏi theo nhiều tiêu chí.
        /// </summary>
        /// <remarks>
        /// Endpoint này hỗ trợ phân trang, tìm kiếm theo từ khóa, lọc theo môn học, chủ đề, độ khó, và sắp xếp.
        /// Ví dụ: `/api/v1/cau-hoi/tim-kiem?Keyword=axit&amp;MonHoc=HoaHoc&amp;PageNumber=1&amp;PageSize=10&amp;SortBy=DoKho&amp;SortDirection=DESC`
        /// </remarks>
        /// <param name="searchParams">Đối tượng chứa các tham số tìm kiếm.</param>
        /// <returns>Danh sách câu hỏi thỏa mãn điều kiện.</returns>
        [HttpGet("tim-kiem")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] CauHoiSearchParametersDTO searchParams)
        {
            try
            {
                var teacherId = HttpContext.GetUserId();
                var result = await _cauHoiService.SearchAsync(teacherId, searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu SortBy không phải là một thuộc tính hợp lệ
                return BadRequest(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThatBai($"Lỗi truy vấn: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách câu hỏi theo một chủ đề cụ thể (có phân trang).
        /// </summary>
        /// <param name="chuDe">Tên chủ đề cần lọc.</param>
        /// <param name="pagingParams">Tham số phân trang và sắp xếp.</param>
        /// <returns>Danh sách câu hỏi thuộc chủ đề đã cho.</returns>
        [HttpGet("chu-de/{chuDe}")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByChuDe(string chuDe, [FromQuery] PagingDTO pagingParams)
        {
            try
            {
                var teacherId = HttpContext.GetUserId();

                // Tái sử dụng DTO tìm kiếm và điền sẵn tham số `ChuDe`
                var searchParams = new CauHoiSearchParametersDTO
                {
                    ChuDe = chuDe,
                    PageNumber = pagingParams.PageNumber,
                    PageSize = pagingParams.PageSize,
                    SortBy = pagingParams.SortBy,
                    SortDirection = pagingParams.SortDirection
                };

                // Gọi đến phương thức tìm kiếm chung
                var result = await _cauHoiService.SearchAsync(teacherId, searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThatBai($"Lỗi truy vấn: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách câu hỏi theo một độ khó cụ thể (có phân trang).
        /// </summary>
        /// <param name="doKho">Độ khó cần lọc (ví dụ: easy, medium, hard).</param>
        /// <param name="pagingParams">Tham số phân trang và sắp xếp.</param>
        /// <returns>Danh sách câu hỏi có độ khó đã cho.</returns>
        [HttpGet("do-kho/{doKho}")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByDoKho(string doKho, [FromQuery] PagingDTO pagingParams)
        {
            try
            {
                var teacherId = HttpContext.GetUserId();

                // Tái sử dụng DTO tìm kiếm và điền sẵn tham số `DoKho`
                var searchParams = new CauHoiSearchParametersDTO
                {
                    DoKho = doKho,
                    PageNumber = pagingParams.PageNumber,
                    PageSize = pagingParams.PageSize,
                    SortBy = pagingParams.SortBy,
                    SortDirection = pagingParams.SortDirection
                };

                // Gọi đến phương thức tìm kiếm chung
                var result = await _cauHoiService.SearchAsync(teacherId, searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThatBai($"Lỗi truy vấn: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách câu hỏi theo một môn học cụ thể (có phân trang).
        /// </summary>
        /// <param name="monHoc">Tên môn học cần lọc (ví dụ: HoaHoc, VatLy).</param>
        /// <param name="pagingParams">Tham số phân trang và sắp xếp.</param>
        /// <returns>Danh sách câu hỏi thuộc môn học đã cho.</returns>
        [HttpGet("mon-hoc/{monHoc}")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByMonHoc(string monHoc, [FromQuery] PagingDTO pagingParams)
        {
            try
            {
                var teacherId = HttpContext.GetUserId();

                // Tái sử dụng DTO tìm kiếm và điền sẵn tham số `MonHoc`
                var searchParams = new CauHoiSearchParametersDTO
                {
                    MonHoc = monHoc,
                    PageNumber = pagingParams.PageNumber,
                    PageSize = pagingParams.PageSize,
                    SortBy = pagingParams.SortBy,
                    SortDirection = pagingParams.SortDirection
                };

                // Gọi đến phương thức tìm kiếm chung
                var result = await _cauHoiService.SearchAsync(teacherId, searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>.ThatBai($"Lỗi truy vấn: {ex.Message}"));
            }
        }


        /// <summary>
        /// Nhập câu hỏi hàng loạt từ một file Excel.
        /// </summary>
        /// <remarks>
        /// File Excel phải có các cột theo thứ tự: 
        /// 1. NoiDung, 2. MonHoc, 3. DoKho, 4. DapAnDung (ký tự A/B/C/D), 
        /// 5. LuaChonA, 6. LuaChonB, 7. LuaChonC, 8. LuaChonD, 9. GiaiThich.
        /// Dòng đầu tiên được coi là header và sẽ bị bỏ qua.
        /// </remarks>
        /// <param name="file">File Excel (.xlsx) chứa danh sách câu hỏi.</param>
        [HttpPost("import-excel")]
        [ProducesResponseType(typeof(ApiPhanHoi<ImportResultDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiPhanHoi<ImportResultDTO>.ThatBai("Không có file nào được tải lên."));
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                return BadRequest(ApiPhanHoi<ImportResultDTO>.ThatBai("Chỉ hỗ trợ file định dạng .xlsx."));
            }

            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.ImportFromExcelAsync(file, teacherId);

            if (result.MaTrangThai != 200)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xuất toàn bộ ngân hàng câu hỏi của giáo viên ra file Excel.
        /// </summary>
        [HttpGet("export-excel")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPhanHoi<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportToExcel()
        {
            var teacherId = HttpContext.GetUserId();
            var fileBytes = await _cauHoiService.ExportToExcelAsync(teacherId);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound(ApiPhanHoi<object>.ThatBai("Không có dữ liệu câu hỏi để xuất file."));
            }

            string fileName = $"NganHangCauHoi_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, mimeType, fileName);
        }

        /// <summary>
        /// Lọc câu hỏi theo độ khó
        /// </summary>
        [HttpGet("loc-do-kho")]
        [ProducesResponseType(typeof(ApiPhanHoi<PagedResult<CauHoiResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByDifficulty([FromQuery] string doKho, [FromQuery] PagingDTO pagingParams)
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _cauHoiService.FilterByDifficultyAsync(teacherId, doKho, pagingParams);
            return Ok(result);
        }
    }
}