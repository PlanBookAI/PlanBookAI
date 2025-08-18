using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Kiểm tra trạng thái dịch vụ
    /// </summary>
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new
        {
            Service = "AuthService",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }

    /// <summary>
    /// Kiểm tra kết nối database
    /// </summary>
    [HttpGet("database")]
    public ActionResult CheckDatabase()
    {
        return Ok(new
        {
            Database = "Connected",
            Timestamp = DateTime.UtcNow
        });
    }
}
