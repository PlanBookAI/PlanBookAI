using GatewayService.Interfaces;

namespace GatewayService.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IJwtService jwtService, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra endpoint có cần authentication không
            if (RequiresAuthentication(context))
            {
                var token = ExtractTokenFromRequest(context.Request);

                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("Processing token for endpoint: {Path}", context.Request.Path);
                    await ProcessTokenAsync(context, token);
                }
                else
                {
                    _logger.LogWarning("No token found for protected endpoint: {Path}", context.Request.Path);
                    context.Items["AuthenticationError"] = "Token không được cung cấp";
                    // Không return sớm, vẫn forward request để downstream service xử lý
                }
            }
            else
            {
                _logger.LogDebug("Public endpoint, no authentication required: {Path}", context.Request.Path);
            }

            await _next(context);
        }

        private string? ExtractTokenFromRequest(HttpRequest request)
        {
            // Kiểm tra Authorization header
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            // Kiểm tra query parameter (cho websocket hoặc các trường hợp đặc biệt)
            var tokenFromQuery = request.Query["token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenFromQuery))
            {
                return tokenFromQuery;
            }

            return null;
        }

        private async Task ProcessTokenAsync(HttpContext context, string token)
        {
            try
            {
                // Kiểm tra token hết hạn
                if (_jwtService.IsTokenExpired(token))
                {
                    _logger.LogWarning("Expired token detected for request to {Path}", context.Request.Path);
                    context.Items["AuthenticationError"] = "Token đã hết hạn";
                    // Không return sớm, vẫn forward request để downstream service xử lý
                }

                // Validate token và lấy user ID
                var userId = await _jwtService.ValidateTokenAsync(token);
                if (!string.IsNullOrEmpty(userId))
                {
                    // Lưu thông tin user vào context
                    context.Items["UserId"] = userId;
                    context.Items["IsAuthenticated"] = true;

                    // Lấy thêm thông tin user claims
                    var userClaims = await _jwtService.GetUserClaimsAsync(token);
                    if (userClaims != null)
                    {
                        context.Items["UserClaims"] = userClaims;
                        
                        // Lưu role nếu có
                        if (userClaims.TryGetValue("role", out var role) || 
                            userClaims.TryGetValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out role))
                        {
                            context.Items["UserRole"] = role;
                        }
                    }

                    // Thêm headers cho downstream services
                    // Sử dụng Append thay vì set trực tiếp để tránh conflict với YARP
                    if (!context.Request.Headers.ContainsKey("X-User-Id"))
                    {
                        context.Request.Headers.Append("X-User-Id", userId);
                    }
                    
                    string? userRole = null;
                    if (userClaims?.TryGetValue("role", out var roleValue) == true)
                    {
                        userRole = roleValue;
                        if (!context.Request.Headers.ContainsKey("X-User-Role"))
                        {
                            context.Request.Headers.Append("X-User-Role", roleValue);
                        }
                    }
                    
                    // Log headers để debug
                    _logger.LogInformation("Set headers: X-User-Id={UserId}, X-User-Role={UserRole}", 
                                         userId, userRole ?? "N/A");

                    _logger.LogDebug("Token validation successful for user {UserId} accessing {Path}", 
                                   userId, context.Request.Path);
                }
                else
                {
                    _logger.LogWarning("Invalid token for request to {Path}", context.Request.Path);
                    context.Items["AuthenticationError"] = "Token không hợp lệ";
                    // Không return sớm, vẫn forward request để downstream service xử lý
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing JWT token for request to {Path}", context.Request.Path);
                context.Items["AuthenticationError"] = "Lỗi xác thực token";
                // Không set response, vẫn forward request để downstream service xử lý
            }
        }



        /// <summary>
        /// Kiểm tra endpoint có cần authentication không
        /// </summary>
        private bool RequiresAuthentication(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();
            
            // Danh sách endpoints không cần authentication
            var publicEndpoints = new[]
            {
                "/health",
                "/swagger",
                "/api/v1/xac-thuc/dang-nhap",
                "/api/v1/xac-thuc/dang-ky"
            };

            return !publicEndpoints.Any(endpoint => path?.StartsWith(endpoint) == true);
        }
    }

    // Extension method để dễ dàng sử dụng middleware
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
