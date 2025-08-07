using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra trạng thái sức khỏe của Gateway Service
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
                    Service = "Gateway Service",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Message = "PlanbookAI Gateway Service đang hoạt động bình thường"
                };

                _logger.LogInformation("Health check requested - Service is healthy");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Gateway Service",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái chi tiết của Gateway và các downstream services
        /// </summary>
        /// <returns>Thông tin chi tiết về trạng thái các services</returns>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            try
            {
                var detailedStatus = new
                {
                    Status = "Healthy",
                    Service = "Gateway Service",
                    Version = "1.0.0",
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    DownstreamServices = new
                    {
                        DichVuXacThuc = new { Url = "http://localhost:8081", Status = "Configured", Route = "/api/v1/xac-thuc" },
                        DichVuNguoiDung = new { Url = "http://localhost:8082", Status = "Configured", Route = "/api/v1/nguoi-dung" },
                        DichVuGiaoAn = new { Url = "http://localhost:8083", Status = "Configured", Route = "/api/v1/giao-an" },
                        DichVuCauHoi = new { Url = "http://localhost:8084", Status = "Configured", Route = "/api/v1/cau-hoi" }
                    },
                    Configuration = new
                    {
                        YarpEnabled = true,
                        CorsEnabled = true,
                        HealthChecksEnabled = true
                    }
                };

                _logger.LogInformation("Detailed health check requested");
                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Service = "Gateway Service",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
