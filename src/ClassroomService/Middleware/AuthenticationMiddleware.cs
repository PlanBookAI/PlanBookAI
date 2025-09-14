using System.Security.Claims;

namespace ClassroomService.Middleware
{
    /// <summary>
    /// Middleware for handling authentication headers from API Gateway
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of AuthenticationMiddleware
        /// </summary>
        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes authentication headers and validates user information
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for health checks and swagger
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
            var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                _logger.LogWarning("Missing authentication headers");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Thiếu thông tin xác thực");
                return;
            }

            // Add user information to HttpContext
            var claims = new List<Claim>
            {
                new Claim("userId", userId),
                new Claim("userRole", userRole)
            };

            if (!string.IsNullOrEmpty(userEmail))
            {
                claims.Add(new Claim("userEmail", userEmail));
            }

            var identity = new ClaimsIdentity(claims, "Gateway");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            await _next(context);
        }
    }
}