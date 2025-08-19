using Microsoft.AspNetCore.Http;

namespace UserService.Middleware
{
    public class HeaderAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HeaderAuthenticationMiddleware> _logger;

        public HeaderAuthenticationMiddleware(RequestDelegate next, ILogger<HeaderAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Đọc headers từ GatewayService
                var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
                var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
                var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();

                // Log tất cả headers để debug
                _logger.LogInformation("All headers received: {Headers}", 
                                     string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}")));

                if (!string.IsNullOrEmpty(userId))
                {
                    // Lưu thông tin user vào HttpContext.Items để controller có thể truy cập
                    context.Items["UserId"] = userId;
                    context.Items["UserRole"] = userRole;
                    context.Items["UserEmail"] = userEmail;
                    context.Items["IsAuthenticated"] = true;

                    _logger.LogInformation("User authenticated: {UserId}, Role: {UserRole}, Email: {UserEmail}", 
                                         userId, userRole ?? "N/A", userEmail ?? "N/A");
                }
                else
                {
                    context.Items["IsAuthenticated"] = false;
                    _logger.LogWarning("No authentication headers found. Headers: {Headers}", 
                                     string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}")));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HeaderAuthenticationMiddleware");
                context.Items["IsAuthenticated"] = false;
            }

            await _next(context);
        }
    }

    public static class HeaderAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseHeaderAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderAuthenticationMiddleware>();
        }
    }
}
