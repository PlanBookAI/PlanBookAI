using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
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
        /// Kiểm tra trạng thái sức khỏe của User Service
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
                    Service = "User Service (Dịch vụ Người dùng)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8082",
                    Routes = new[]
                    {
                        "/api/v1/nguoi-dung/ho-so",
                        "/api/v1/nguoi-dung/cap-nhat-thong-tin",
                        "/api/v1/nguoi-dung/doi-mat-khau",
                        "/api/v1/nguoi-dung/quan-ly" // Admin only
                    },
                    Message = "Dịch vụ Người dùng đang hoạt động bình thường"
                };

                _logger.LogInformation("Health check requested - User Service is healthy");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for User Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "User Service (Dịch vụ Người dùng)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái chi tiết của User Service
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
                    Service = "User Service (Dịch vụ Người dùng)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8082",
                    Features = new
                    {
                        ProfileManagement = true,
                        UserAdministration = true,
                        ActivityLogging = true,
                        RoleManagement = true
                    },
                    Dependencies = new
                    {
                        Database = "Ready", // Sẽ cập nhật khi có DB connection
                        AuthService = "External", // Dependency on AuthService
                        FileStorage = "Ready"
                    },
                    Configuration = new
                    {
                        DatabaseEnabled = false, // Sẽ cập nhật khi có DB
                        FileUploadEnabled = true,
                        LoggingEnabled = true,
                        AdminFeaturesEnabled = true
                    }
                };

                _logger.LogInformation("Detailed health check requested for User Service");
                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed for User Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "User Service (Dịch vụ Người dùng)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
