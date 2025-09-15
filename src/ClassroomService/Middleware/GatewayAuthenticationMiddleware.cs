using System.Security.Claims;

namespace ClassroomService.Middleware
{
    /// <summary>
    /// Middleware to handle Gateway authentication headers
    /// </summary>
    public class GatewayAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayAuthenticationMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of GatewayAuthenticationMiddleware
        /// </summary>
        public GatewayAuthenticationMiddleware(RequestDelegate next, ILogger<GatewayAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Extract user information from Gateway headers
                var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
                var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
                var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();
                var userName = context.Request.Headers["X-User-Name"].FirstOrDefault();

                if (!string.IsNullOrEmpty(userId))
                {
                    // Create claims identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Name, userName ?? string.Empty),
                        new Claim(ClaimTypes.Email, userEmail ?? string.Empty),
                        new Claim(ClaimTypes.Role, userRole ?? "TEACHER")
                    };

                    var identity = new ClaimsIdentity(claims, "Gateway");
                    var principal = new ClaimsPrincipal(identity);

                    context.User = principal;

                    _logger.LogInformation("Gateway authentication successful for user {UserId} with role {UserRole}", userId, userRole);
                }
                else
                {
                    _logger.LogWarning("No X-User-Id header found in request");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Gateway authentication middleware");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension methods for Gateway authentication middleware
    /// </summary>
    public static class GatewayAuthenticationMiddlewareExtensions
    {
        /// <summary>
        /// Adds Gateway authentication middleware to the pipeline
        /// </summary>
        public static IApplicationBuilder UseGatewayAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GatewayAuthenticationMiddleware>();
        }
    }
}
