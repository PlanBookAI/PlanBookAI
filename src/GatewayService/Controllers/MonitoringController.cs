using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/v1/giam-sat")]
public class MonitoringController : ControllerBase
{
    private readonly ILogger<MonitoringController> _logger;
    private readonly IConfiguration _configuration;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public MonitoringController(ILogger<MonitoringController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("thong-tin-he-thong")]
    public ActionResult<SystemInfoResponse> GetSystemInfo()
    {
        var process = Process.GetCurrentProcess();
        var assembly = Assembly.GetExecutingAssembly();
        
        var systemInfo = new SystemInfoResponse
        {
            ServiceName = "PlanbookAI Gateway Service",
            Version = assembly.GetName().Version?.ToString() ?? "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            StartTime = _startTime,
            Uptime = DateTime.UtcNow - _startTime,
            ProcessId = process.Id,
            MachineName = Environment.MachineName,
            WorkingSet = process.WorkingSet64,
            PrivateMemorySize = process.PrivateMemorySize64,
            ThreadCount = process.Threads.Count,
            HandleCount = process.HandleCount
        };

        return Ok(systemInfo);
    }

    [HttpGet("thong-ke")]
    public ActionResult<StatisticsResponse> GetStatistics()
    {
        // Trong thực tế, bạn sẽ lấy dữ liệu từ cache hoặc database
        var statistics = new StatisticsResponse
        {
            TotalRequests = GetTotalRequestsFromCache(),
            RequestsPerMinute = GetRequestsPerMinuteFromCache(),
            AverageResponseTime = GetAverageResponseTimeFromCache(),
            ErrorRate = GetErrorRateFromCache(),
            ActiveConnections = GetActiveConnectionsFromCache(),
            LastUpdated = DateTime.UtcNow
        };

        return Ok(statistics);
    }

    [HttpGet("cau-hinh")]
    public ActionResult<ConfigurationResponse> GetConfiguration()
    {
        var config = new ConfigurationResponse
        {
            RateLimiting = new RateLimitingConfig
            {
                MaxRequests = _configuration.GetValue<int>("RateLimiting:MaxRequests"),
                WindowSizeInMinutes = _configuration.GetValue<int>("RateLimiting:WindowSizeInMinutes")
            },
            Cors = new CorsConfig
            {
                AllowedOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" },
                AllowedMethods = _configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE" },
                AllowCredentials = _configuration.GetValue<bool>("Cors:AllowCredentials")
            },
            Services = GetServiceEndpoints()
        };

        return Ok(config);
    }

    [HttpGet("kiem-tra-dich-vu")]
    public async Task<ActionResult<ServiceHealthResponse>> CheckServiceHealth()
    {
        var services = new List<ServiceHealth>();
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        // Danh sách các service cần kiểm tra
        var serviceEndpoints = new Dictionary<string, string>
        {
            { "auth-service", "http://host.docker.internal:8081/health" },
            { "user-service", "http://host.docker.internal:8082/health" },
            { "plan-service", "http://host.docker.internal:8083/health" },
            { "task-service", "http://host.docker.internal:8084/health" }
        };

        foreach (var service in serviceEndpoints)
        {
            var serviceHealth = new ServiceHealth
            {
                ServiceName = service.Key,
                Endpoint = service.Value,
                Status = "Healthy",
                ResponseTime = 0,
                LastChecked = DateTime.UtcNow
            };

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.GetAsync(service.Value);
                stopwatch.Stop();

                serviceHealth.ResponseTime = stopwatch.ElapsedMilliseconds;
                serviceHealth.Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
                serviceHealth.StatusCode = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                serviceHealth.Status = "Unhealthy";
                serviceHealth.Error = ex.Message;
                _logger.LogWarning("Service {ServiceName} health check failed: {Error}", service.Key, ex.Message);
            }

            services.Add(serviceHealth);
        }

        var overallStatus = services.All(s => s.Status == "Healthy") ? "Healthy" : "Unhealthy";

        var healthResponse = new ServiceHealthResponse
        {
            OverallStatus = overallStatus,
            Services = services,
            CheckedAt = DateTime.UtcNow
        };

        return Ok(healthResponse);
    }

    [HttpPost("xoa-cache")]
    public ActionResult ClearCache()
    {
        // Trong thực tế, bạn sẽ xóa cache ở đây
        _logger.LogInformation("Cache cleared by admin request");
        return Ok(new { message = "Cache đã được xóa thành công", timestamp = DateTime.UtcNow });
    }

    // Helper methods (trong thực tế sẽ lấy từ cache/database)
    private long GetTotalRequestsFromCache() => 0; // Placeholder
    private double GetRequestsPerMinuteFromCache() => 0.0; // Placeholder
    private double GetAverageResponseTimeFromCache() => 0.0; // Placeholder
    private double GetErrorRateFromCache() => 0.0; // Placeholder
    private int GetActiveConnectionsFromCache() => 0; // Placeholder

    private List<ServiceEndpoint> GetServiceEndpoints()
    {
        return new List<ServiceEndpoint>
        {
            new ServiceEndpoint { Name = "xac-thuc", Url = "http://host.docker.internal:8081", Path = "/api/v1/xac-thuc" },
            new ServiceEndpoint { Name = "nguoi-dung", Url = "http://host.docker.internal:8082", Path = "/api/v1/nguoi-dung" },
            new ServiceEndpoint { Name = "giao-an", Url = "http://host.docker.internal:8083", Path = "/api/v1/giao-an" },
            new ServiceEndpoint { Name = "nhiem-vu", Url = "http://host.docker.internal:8084", Path = "/api/v1/nhiem-vu" }
        };
    }
}

// Response Models
public class SystemInfoResponse
{
    public string ServiceName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public TimeSpan Uptime { get; set; }
    public int ProcessId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public long WorkingSet { get; set; }
    public long PrivateMemorySize { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
}

public class StatisticsResponse
{
    public long TotalRequests { get; set; }
    public double RequestsPerMinute { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public int ActiveConnections { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ConfigurationResponse
{
    public RateLimitingConfig RateLimiting { get; set; } = new();
    public CorsConfig Cors { get; set; } = new();
    public List<ServiceEndpoint> Services { get; set; } = new();
}

public class RateLimitingConfig
{
    public int MaxRequests { get; set; }
    public int WindowSizeInMinutes { get; set; }
}

public class CorsConfig
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();
    public bool AllowCredentials { get; set; }
}

public class ServiceEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

public class ServiceHealthResponse
{
    public string OverallStatus { get; set; } = string.Empty;
    public List<ServiceHealth> Services { get; set; } = new();
    public DateTime CheckedAt { get; set; }
}

public class ServiceHealth
{
    public string ServiceName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long ResponseTime { get; set; }
    public int? StatusCode { get; set; }
    public string? Error { get; set; }
    public DateTime LastChecked { get; set; }
}
