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

        // Danh sách tất cả 11 services cần kiểm tra
        var serviceEndpoints = new Dictionary<string, string>
        {
            { "auth-service", "http://host.docker.internal:8081/api/v1/health" },
            { "user-service", "http://host.docker.internal:8082/api/v1/health" },
            { "plan-service", "http://host.docker.internal:8083/api/v1/health" },
            { "exam-service", "http://host.docker.internal:8084/api/v1/health" },
            { "classroom-service", "http://host.docker.internal:8085/api/v1/health" },
            { "file-storage-service", "http://host.docker.internal:8086/api/v1/health" },
            { "notification-service", "http://host.docker.internal:8087/api/v1/health" },
            { "ocr-service", "http://host.docker.internal:8088/api/v1/health" },
            { "student-grading-service", "http://host.docker.internal:8089/api/v1/health" },
            { "ai-plan-service", "http://host.docker.internal:8090/api/v1/health" },
            { "log-service", "http://host.docker.internal:8091/api/v1/health" }
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

    [HttpGet("kiem-tra-dich-vu-qua-gateway")]
    public async Task<ActionResult<ServiceHealthResponse>> CheckServiceHealthViaGateway()
    {
        var services = new List<ServiceHealth>();
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        // Danh sách health endpoints qua Gateway
        var gatewayHealthEndpoints = new Dictionary<string, string>
        {
            { "auth-service", "http://localhost:8080/api/v1/xac-thuc-health" },
            { "user-service", "http://localhost:8080/api/v1/nguoi-dung-health" },
            { "plan-service", "http://localhost:8080/api/v1/giao-an-health" },
            { "exam-service", "http://localhost:8080/api/v1/de-thi-health" },
            { "classroom-service", "http://localhost:8080/api/v1/lop-hoc-health" },
            { "file-storage-service", "http://localhost:8080/api/v1/file-storage-health" },
            { "notification-service", "http://localhost:8080/api/v1/thong-bao-health" },
            { "ocr-service", "http://localhost:8080/api/v1/ocr-health" },
            { "student-grading-service", "http://localhost:8080/api/v1/hoc-sinh-health" },
            { "ai-plan-service", "http://localhost:8080/api/v1/ai-plan-health" },
            { "log-service", "http://localhost:8080/api/v1/log-service-health" }
        };

        foreach (var service in gatewayHealthEndpoints)
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
                _logger.LogWarning("Service {ServiceName} gateway health check failed: {Error}", service.Key, ex.Message);
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

    [HttpGet("trang-thai-he-thong")]
    public ActionResult<SystemStatusResponse> GetSystemStatus()
    {
        var systemStatus = new SystemStatusResponse
        {
            GatewayStatus = "Healthy",
            TotalServices = 11,
            HealthyServices = 11, // Sẽ được cập nhật từ health check
            UnhealthyServices = 0,
            LastHealthCheck = DateTime.UtcNow,
            SystemUptime = DateTime.UtcNow - _startTime,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        return Ok(systemStatus);
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
            new ServiceEndpoint { Name = "xac-thuc", Url = "http://host.docker.internal:8081", Path = "/api/v1/xac-thuc", Port = 8081 },
            new ServiceEndpoint { Name = "nguoi-dung", Url = "http://host.docker.internal:8082", Path = "/api/v1/nguoi-dung", Port = 8082 },
            new ServiceEndpoint { Name = "giao-an", Url = "http://host.docker.internal:8083", Path = "/api/v1/giao-an", Port = 8083 },
            new ServiceEndpoint { Name = "de-thi", Url = "http://host.docker.internal:8084", Path = "/api/v1/de-thi", Port = 8084 },
            new ServiceEndpoint { Name = "lop-hoc", Url = "http://host.docker.internal:8085", Path = "/api/v1/lop-hoc", Port = 8085 },
            new ServiceEndpoint { Name = "file-storage", Url = "http://host.docker.internal:8086", Path = "/api/v1/file-storage", Port = 8086 },
            new ServiceEndpoint { Name = "thong-bao", Url = "http://host.docker.internal:8087", Path = "/api/v1/thong-bao", Port = 8087 },
            new ServiceEndpoint { Name = "ocr", Url = "http://host.docker.internal:8088", Path = "/api/v1/ocr", Port = 8088 },
            new ServiceEndpoint { Name = "hoc-sinh", Url = "http://host.docker.internal:8089", Path = "/api/v1/hoc-sinh", Port = 8089 },
            new ServiceEndpoint { Name = "ai-plan", Url = "http://host.docker.internal:8090", Path = "/api/v1/ai-plan", Port = 8090 },
            new ServiceEndpoint { Name = "log-service", Url = "http://host.docker.internal:8091", Path = "/api/v1/log-service", Port = 8091 }
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
    public int Port { get; set; }
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

public class SystemStatusResponse
{
    public string GatewayStatus { get; set; } = string.Empty;
    public int TotalServices { get; set; }
    public int HealthyServices { get; set; }
    public int UnhealthyServices { get; set; }
    public DateTime LastHealthCheck { get; set; }
    public TimeSpan SystemUptime { get; set; }
    public string Environment { get; set; } = string.Empty;
}
