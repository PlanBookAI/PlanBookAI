using Microsoft.AspNetCore.Mvc;

namespace ClassroomService.Controllers;

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
                Service = "ClassroomService",
                Features = new
                {
                    ClassManagement = "Available",
                    LessonPlanLinking = "Available",
                    ProgressTracking = "Available",
                    ScheduleManagement = "Available"
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
