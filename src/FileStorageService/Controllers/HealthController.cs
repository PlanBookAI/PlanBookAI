using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.Controllers;

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
                Service = "FileStorageService",
                Features = new
                {
                    FileUpload = "Available",
                    FileDownload = "Available",
                    StorageManagement = "Available"
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
