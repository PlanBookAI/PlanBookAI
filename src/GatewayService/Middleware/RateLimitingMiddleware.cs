using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace GatewayService.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly IMemoryCache _cache;
        private readonly RateLimitingOptions _options;

        public RateLimitingMiddleware(
            RequestDelegate next, 
            ILogger<RateLimitingMiddleware> logger,
            IMemoryCache cache,
            RateLimitingOptions options)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip rate limiting for health checks and static files
            if (ShouldSkipRateLimit(context))
            {
                await _next(context);
                return;
            }

            var clientId = GetClientIdentifier(context);
            var rateLimitKey = $"rate_limit:{clientId}";

            // Get current request count
            var requestInfo = _cache.Get<RequestInfo>(rateLimitKey) ?? new RequestInfo();

            // Clean up old requests (older than window)
            var windowStart = DateTime.UtcNow.AddMinutes(-_options.WindowSizeInMinutes);
            requestInfo.Requests.RemoveAll(r => r < windowStart);

            // Check if limit exceeded
            if (requestInfo.Requests.Count >= _options.MaxRequests)
            {
                await HandleRateLimitExceeded(context, clientId, requestInfo);
                return;
            }

            // Add current request
            requestInfo.Requests.Add(DateTime.UtcNow);
            requestInfo.LastRequest = DateTime.UtcNow;

            // Update cache
            _cache.Set(rateLimitKey, requestInfo, TimeSpan.FromMinutes(_options.WindowSizeInMinutes + 1));

            // Add rate limit headers
            AddRateLimitHeaders(context, requestInfo);

            // Log rate limit info
            LogRateLimit(context, clientId, requestInfo);

            await _next(context);
        }

        private bool ShouldSkipRateLimit(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();
            
            var skipPaths = new[]
            {
                "/health",
                "/swagger",
                "/favicon.ico",
                "/.well-known"
            };

            return skipPaths.Any(skipPath => path?.StartsWith(skipPath) == true);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Priority: User ID > IP Address
            var userId = context.Items["UserId"]?.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                return $"user:{userId}";
            }

            // Use IP address as fallback
            var clientIP = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ??
                          context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                          context.Connection.RemoteIpAddress?.ToString() ??
                          "unknown";

            return $"ip:{clientIP}";
        }

        private async Task HandleRateLimitExceeded(HttpContext context, string clientId, RequestInfo requestInfo)
        {
            var remainingTime = _options.WindowSizeInMinutes * 60 - (int)(DateTime.UtcNow - requestInfo.Requests.First()).TotalSeconds;
            
            _logger.LogWarning("🚫 RATE LIMIT EXCEEDED | Client: {ClientId} | Requests: {RequestCount}/{MaxRequests} | Reset in: {RemainingTime}s",
                clientId, requestInfo.Requests.Count, _options.MaxRequests, remainingTime);

            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.ContentType = "application/json";

            // Add rate limit headers
            context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddSeconds(remainingTime).ToUnixTimeSeconds().ToString();
            context.Response.Headers["Retry-After"] = remainingTime.ToString();

            var response = new
            {
                error = "RATE_LIMIT_EXCEEDED",
                message = $"Đã vượt quá giới hạn {_options.MaxRequests} requests trong {_options.WindowSizeInMinutes} phút",
                limit = _options.MaxRequests,
                windowSizeMinutes = _options.WindowSizeInMinutes,
                resetTime = DateTimeOffset.UtcNow.AddSeconds(remainingTime).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                retryAfterSeconds = remainingTime
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }

        private void AddRateLimitHeaders(HttpContext context, RequestInfo requestInfo)
        {
            var remaining = Math.Max(0, _options.MaxRequests - requestInfo.Requests.Count);
            var resetTime = requestInfo.Requests.Count > 0 
                ? requestInfo.Requests.First().AddMinutes(_options.WindowSizeInMinutes)
                : DateTime.UtcNow.AddMinutes(_options.WindowSizeInMinutes);

            context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = new DateTimeOffset(resetTime).ToUnixTimeSeconds().ToString();
        }

        private void LogRateLimit(HttpContext context, string clientId, RequestInfo requestInfo)
        {
            var userId = context.Items["UserId"]?.ToString() ?? "Anonymous";
            var remaining = Math.Max(0, _options.MaxRequests - requestInfo.Requests.Count);

            _logger.LogDebug("⚡ RATE LIMIT | Client: {ClientId} | User: {UserId} | Requests: {RequestCount}/{MaxRequests} | Remaining: {Remaining}",
                clientId, userId, requestInfo.Requests.Count, _options.MaxRequests, remaining);

            // Warn when approaching limit
            if (remaining <= 10 && remaining > 0)
            {
                _logger.LogWarning("⚠️ RATE LIMIT WARNING | Client: {ClientId} | Only {Remaining} requests remaining",
                    clientId, remaining);
            }
        }
    }

    public class RequestInfo
    {
        public List<DateTime> Requests { get; set; } = new();
        public DateTime LastRequest { get; set; }
    }

    public class RateLimitingOptions
    {
        public int MaxRequests { get; set; } = 100;
        public int WindowSizeInMinutes { get; set; } = 1;
    }

    // Extension method
    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, RateLimitingOptions? options = null)
        {
            options ??= new RateLimitingOptions();
            return builder.UseMiddleware<RateLimitingMiddleware>(options);
        }
    }
}
