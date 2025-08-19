using Microsoft.AspNetCore.Mvc;

namespace StudentGradingService.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    public HealthController()
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            // Kiểm tra service status
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "StudentGradingService",
                Features = new
                {
                    StudentManagement = "Available",
                    ClassManagement = "Available",
                    ProgressTracking = "Available",
                    ExcelImport = "Available"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }
}
