using Microsoft.AspNetCore.Mvc;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra trạng thái sức khỏe của Task Service
        /// </summary>
        /// <returns>Thông tin trạng thái service</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var healthStatus = new
                {
                    Status = "Healthy",
                    Service = "Task Service (Dịch vụ Câu hỏi & Đề thi)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8084",
                    Routes = new[]
                    {
                        "/api/v1/cau-hoi/tao",
                        "/api/v1/cau-hoi/danh-sach",
                        "/api/v1/cau-hoi/cap-nhat/{id}",
                        "/api/v1/cau-hoi/xoa/{id}",
                        "/api/v1/de-thi/tao",
                        "/api/v1/de-thi/danh-sach",
                        "/api/v1/ket-qua/cham-diem",
                        "/api/v1/hoc-sinh/quan-ly"
                    },
                    Message = "Dịch vụ Câu hỏi & Đề thi đang hoạt động bình thường"
                };

                _logger.LogInformation("Health check requested - Task Service is healthy");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for Task Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Task Service (Dịch vụ Câu hỏi & Đề thi)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái chi tiết của Task Service
        /// </summary>
        /// <returns>Thông tin chi tiết về trạng thái service</returns>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            try
            {
                var detailedStatus = new
                {
                    Status = "Healthy",
                    Service = "Task Service (Dịch vụ Câu hỏi & Đề thi)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8084",
                    Features = new
                    {
                        QuestionBankManagement = true,
                        ExamGeneration = true,
                        OCRGrading = true, // Google Vision API
                        StudentDataManagement = true,
                        ResultsAnalytics = true,
                        MultipleChoiceSupport = true,
                        AutomaticScoring = true
                    },
                    Dependencies = new
                    {
                        Database = "Ready", // Sẽ cập nhật khi có DB connection
                        GoogleVisionAPI = "External", // OCR service dependency
                        AuthService = "External", // Authentication dependency
                        FileStorage = "Ready", // For image processing
                        ImageProcessing = "Ready"
                    },
                    Configuration = new
                    {
                        DatabaseEnabled = false, // Sẽ cập nhật khi có DB
                        OCREnabled = false, // Sẽ cập nhật khi có OCR integration
                        FileUploadEnabled = true,
                        LoggingEnabled = true,
                        AnalyticsEnabled = true
                    },
                    SupportedQuestionTypes = new[]
                    {
                        "Trắc nghiệm", // Multiple choice
                        "Đúng/Sai", // True/False
                        "Tự luận", // Essay (future)
                        "Điền khuyết" // Fill in the blank (future)
                    },
                    SupportedSubjects = new[]
                    {
                        "Hóa học", // Primary focus
                        "Vật lý", // Future
                        "Sinh học", // Future
                        "Toán học"  // Future
                    }
                };

                _logger.LogInformation("Detailed health check requested for Task Service");
                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed for Task Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Task Service (Dịch vụ Câu hỏi & Đề thi)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
