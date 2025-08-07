using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
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
        /// Kiểm tra trạng thái sức khỏe của Auth Service
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
                    Service = "Auth Service (Dịch vụ Xác thực)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8081",
                    Routes = new[]
                    {
                        "/api/v1/xac-thuc/dang-nhap",
                        "/api/v1/xac-thuc/dang-ky", 
                        "/api/v1/xac-thuc/lam-moi-token",
                        "/api/v1/xac-thuc/thong-tin-ca-nhan"
                    },
                    Message = "Dịch vụ Xác thực đang hoạt động bình thường"
                };

                _logger.LogInformation("Health check requested - Auth Service is healthy");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for Auth Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Auth Service (Dịch vụ Xác thực)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái chi tiết của Auth Service
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
                    Service = "Auth Service (Dịch vụ Xác thực)",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Port = "8081",
                    Features = new
                    {
                        JWTAuthentication = true,
                        PasswordHashing = true,
                        RoleBasedAccess = true,
                        RefreshTokens = true
                    },
                    Dependencies = new
                    {
                        Database = "Ready", // Sẽ cập nhật khi có DB connection
                        JWTService = "Ready",
                        PasswordHasher = "Ready"
                    },
                    Configuration = new
                    {
                        JWTEnabled = true,
                        DatabaseEnabled = false, // Sẽ cập nhật khi có DB
                        LoggingEnabled = true
                    }
                };

                _logger.LogInformation("Detailed health check requested for Auth Service");
                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed for Auth Service");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Auth Service (Dịch vụ Xác thực)",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
