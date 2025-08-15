using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

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
                // Đọc headers từ Gateway
                var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
                var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
                var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();

                _logger.LogDebug("Headers received: X-User-Id={UserId}, X-User-Role={UserRole}, X-User-Email={UserEmail}", 
                               userId, userRole, userEmail);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Tạo claims cho user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId), // userId là email
                        new Claim(ClaimTypes.Name, userEmail ?? userId),
                        new Claim("UserId", userId), // Thêm claim riêng cho UserId
                        new Claim(ClaimTypes.Email, userId) // Thêm email claim
                    };

                    if (!string.IsNullOrEmpty(userRole))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    // Tạo identity và principal
                    var identity = new ClaimsIdentity(claims, "Gateway");
                    var principal = new ClaimsPrincipal(identity);

                    // Set User cho context
                    context.User = principal;

                    _logger.LogInformation("User authenticated via headers: {UserId} with role {UserRole}", userId, userRole ?? "N/A");
                }
                else
                {
                    _logger.LogWarning("No X-User-Id header found, request will be anonymous");
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HeaderAuthenticationMiddleware");
                await _next(context);
            }
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
