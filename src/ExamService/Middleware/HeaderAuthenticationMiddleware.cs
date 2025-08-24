using System.Security.Claims;

namespace ExamService.Middleware
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
            // Lấy thông tin user từ headers được set bởi Gateway
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
            var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userId))
            {
                var claims = new List<Claim>
                {
                    new Claim("UserId", userId),
                    new Claim(ClaimTypes.NameIdentifier, userEmail ?? userId),
                    new Claim(ClaimTypes.Name, userEmail ?? userId),
                };

                if (!string.IsNullOrEmpty(userRole))
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                if (!string.IsNullOrEmpty(userEmail))
                {
                    claims.Add(new Claim(ClaimTypes.Email, userEmail));
                }

                var identity = new ClaimsIdentity(claims, "HeaderAuthentication");
                var principal = new ClaimsPrincipal(identity);
                context.User = principal;

                _logger.LogDebug("User authenticated from headers: UserId={UserId}, Role={Role}, Email={Email}",
                    userId, userRole, userEmail);
            }
            else
            {
                _logger.LogWarning("No X-User-Id header found, request will be anonymous");
            }

            await _next(context);
        }
    }
}
