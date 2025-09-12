namespace PlanbookAI.FileStorageService.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayAuthMiddleware> _logger;
        
        public GatewayAuthMiddleware(RequestDelegate next, ILogger<GatewayAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for health endpoint and swagger
            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (path?.Contains("/health") == true || 
                path?.Contains("/swagger") == true ||
                path?.Contains("/api-docs") == true)
            {
                await _next(context);
                return;
            }
            
            // Check required headers from Gateway
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
            var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { thongBao = "Chua xac thuc" });
                return;
            }
            
            if (!Guid.TryParse(userId, out _))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { thongBao = "User ID khong hop le" });
                return;
            }
            
            _logger.LogDebug("User authenticated: {UserId}, Role: {UserRole}", userId, userRole);
            
            await _next(context);
        }
    }
}
