using ExamService.Extensions;
using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExamService.Controllers
{
    [ApiController]
    [Route("api/v1/thong-ke")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;

        public ThongKeController(IThongKeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy báo cáo thống kê tổng quan về ngân hàng câu hỏi và đề thi của giáo viên.
        /// </summary>
        /// <returns>Đối tượng chứa các chỉ số thống kê.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiPhanHoi<ThongKeTongQuanDTO>), 200)]
        public async Task<IActionResult> GetGeneralStatistics()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetGeneralStatisticsAsync(teacherId);
            return Ok(result);
        }

        // Các endpoint con có thể được xem là đã được đáp ứng bởi endpoint chung ở trên.
        // Ví dụ, client có thể lấy ThongKeTheoMonHoc từ kết quả của endpoint GET /

        /// <summary>
        /// Lấy báo cáo thống kê chi tiết về ngân hàng câu hỏi của giáo viên.
        /// </summary>
        /// <returns>Đối tượng chứa các chỉ số thống kê về câu hỏi.</returns>
        [HttpGet("cau-hoi")]
        [ProducesResponseType(typeof(ApiPhanHoi<CauHoiThongKeTongQuanDTO>), 200)]
        public async Task<IActionResult> GetQuestionStatistics()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetQuestionStatisticsAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy báo cáo thống kê tổng quan về các đề thi của giáo viên.
        /// </summary>
        /// <returns>Đối tượng chứa các chỉ số thống kê về đề thi.</returns>
        [HttpGet("de-thi")]
        [ProducesResponseType(typeof(ApiPhanHoi<DeThiThongKeTongQuanDTO>), 200)]
        public async Task<IActionResult> GetExamStatisticsOverview()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetExamStatisticsOverviewAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thống kê số lượng câu hỏi theo từng độ khó.
        /// </summary>
        /// <returns>Danh sách các nhóm độ khó và số lượng câu hỏi tương ứng.</returns>
        [HttpGet("do-kho")]
        [ProducesResponseType(typeof(ApiPhanHoi<List<ThongKeTheoNhomDTO>>), 200)]
        public async Task<IActionResult> GetStatsByDifficulty()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetStatsByDifficultyAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thống kê số lượng câu hỏi theo từng chủ đề.
        /// </summary>
        /// <returns>Danh sách các chủ đề và số lượng câu hỏi tương ứng.</returns>
        [HttpGet("chu-de")]
        [ProducesResponseType(typeof(ApiPhanHoi<List<ThongKeTheoNhomDTO>>), 200)]
        public async Task<IActionResult> GetStatsByTopic()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetStatsByTopicAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thống kê số lượng câu hỏi theo từng môn học.
        /// </summary>
        /// <returns>Danh sách các môn học và số lượng câu hỏi tương ứng.</returns>
        [HttpGet("mon-hoc")]
        [ProducesResponseType(typeof(ApiPhanHoi<List<ThongKeTheoNhomDTO>>), 200)]
        public async Task<IActionResult> GetStatsBySubject()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetStatsBySubjectAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thống kê về hoạt động của người dùng đang đăng nhập (tổng số câu hỏi, đề thi đã tạo).
        /// </summary>
        /// <returns>Các chỉ số thống kê của người dùng.</returns>
        [HttpGet("nguoi-dung")]
        [ProducesResponseType(typeof(ApiPhanHoi<NguoiDungThongKeDTO>), 200)]
        public async Task<IActionResult> GetUserStatistics()
        {
            var teacherId = HttpContext.GetUserId();
            var result = await _service.GetUserStatisticsAsync(teacherId);
            return Ok(result);
        }
    }
}