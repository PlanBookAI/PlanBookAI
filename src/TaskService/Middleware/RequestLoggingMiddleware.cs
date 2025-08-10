using Microsoft.Extensions.Logging;

namespace TaskService.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request: {Method} {Path} {QueryString}",
            context.Request.Method, context.Request.Path, context.Request.QueryString);
        
        // Enable buffering để có thể seek request body
        context.Request.EnableBuffering();
        
        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            _logger.LogInformation("Request Body: {Body}", body);
            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }
        
        await _next(context);
        _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
    }
}
