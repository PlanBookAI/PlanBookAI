namespace PlanbookAI.FileStorageService.Middleware
{
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
            var startTime = DateTime.UtcNow;
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            
            _logger.LogInformation("Request started: {Method} {Path} from {IP} by {UserId}", 
                context.Request.Method, context.Request.Path, ipAddress, userId);
            
            await _next(context);
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Request completed: {Method} {Path} {StatusCode} in {Duration}ms", 
                context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
        }
    }
}
