using System.Security.Claims;

namespace ClassroomService.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bỏ qua xác thực để kiểm tra health và swagger
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

            // Thêm thông tin người dùng vào HttpContext
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