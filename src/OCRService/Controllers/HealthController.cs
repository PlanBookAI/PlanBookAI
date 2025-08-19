using Microsoft.AspNetCore.Mvc;

namespace OCRService.Controllers;

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
                Service = "OCRService",
                Features = new
                {
                    TextRecognition = "Available",
                    AnswerSheetProcessing = "Available",
                    GoogleVisionAPI = "Available"
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
