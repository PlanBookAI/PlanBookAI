using Microsoft.AspNetCore.Mvc;

namespace PlanService.Controllers
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
        /// Kiểm tra trạng thái sức khỏe của Plan Service
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
                    Service = "Plan Service (Dịch vụ Giáo án)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8083",
                    Routes = new[]
                    {
                        "/api/v1/giao-an/tao",
                        "/api/v1/giao-an/danh-sach",
                        "/api/v1/giao-an/cap-nhat/{id}",
                        "/api/v1/giao-an/xoa/{id}",
                        "/api/v1/giao-an/mau-giao-an"
                    },
                    Message = "Dịch vụ Giáo án đang hoạt động bình thường"
                };

                _logger.LogInformation("Health check requested - Plan Service is healthy");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for Plan Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Plan Service (Dịch vụ Giáo án)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái chi tiết của Plan Service
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
                    Service = "Plan Service (Dịch vụ Giáo án)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8083",
                    Features = new
                    {
                        LessonPlanCreation = true,
                        TemplateManagement = true,
                        AIIntegration = true, // Gemini AI
                        ContentGeneration = true,
                        VersionControl = true
                    },
                    Dependencies = new
                    {
                        Database = "Ready", // Sẽ cập nhật khi có DB connection
                        GeminiAI = "External", // AI service dependency
                        AuthService = "External", // Authentication dependency
                        FileStorage = "Ready"
                    },
                    Configuration = new
                    {
                        DatabaseEnabled = false, // Sẽ cập nhật khi có DB
                        AIEnabled = false, // Sẽ cập nhật khi có AI integration
                        TemplatesEnabled = true,
                        LoggingEnabled = true
                    },
                    SupportedSubjects = new[]
                    {
                        "Hóa học", // Primary focus
                        "Vật lý", // Future
                        "Sinh học", // Future
                        "Toán học"  // Future
                    }
                };

                _logger.LogInformation("Detailed health check requested for Plan Service");
                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed for Plan Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Plan Service (Dịch vụ Giáo án)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
