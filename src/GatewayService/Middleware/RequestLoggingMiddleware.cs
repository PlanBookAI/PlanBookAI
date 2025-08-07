using System.Diagnostics;
using System.Text;

namespace GatewayService.Middleware
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
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            // Log request
            await LogRequestAsync(context, requestId);

            // Capture response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log response
                await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

                // Copy response back to original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
            var request = context.Request;
            var userId = context.Items["UserId"]?.ToString() ?? "Anonymous";
            var userRole = context.Items["UserRole"]?.ToString() ?? "Unknown";

            var requestInfo = new
            {
                RequestId = requestId,
                Timestamp = DateTime.UtcNow,
                Method = request.Method,
                Path = request.Path.Value,
                QueryString = request.QueryString.Value,
                UserAgent = request.Headers["User-Agent"].FirstOrDefault(),
                ClientIP = GetClientIP(context),
                UserId = userId,
                UserRole = userRole,
                ContentType = request.ContentType,
                ContentLength = request.ContentLength,
                Headers = GetSafeHeaders(request.Headers)
            };

            _logger.LogInformation("🔵 REQUEST [{RequestId}] {Method} {Path} | User: {UserId} ({UserRole}) | IP: {ClientIP}",
                requestId, request.Method, request.Path, userId, userRole, GetClientIP(context));

            // Log detailed info at Debug level
            _logger.LogDebug("REQUEST DETAILS [{RequestId}]: {@RequestInfo}", requestId, requestInfo);

            // Log request body for POST/PUT (but not for file uploads)
            if ((request.Method == "POST" || request.Method == "PUT") &&
                request.ContentLength > 0 &&
                request.ContentLength < 10240 && // Max 10KB
                request.ContentType?.Contains("application/json") == true)
            {
                request.EnableBuffering();
                var body = await ReadRequestBodyAsync(request);
                if (!string.IsNullOrEmpty(body))
                {
                    _logger.LogDebug("REQUEST BODY [{RequestId}]: {Body}", requestId, body);
                }
                request.Body.Position = 0;
            }
        }

        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
        {
            var response = context.Response;
            var userId = context.Items["UserId"]?.ToString() ?? "Anonymous";

            // Read response body
            var responseBody = "";
            if (response.Body.CanSeek && response.Body.Length > 0 && response.Body.Length < 10240) // Max 10KB
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            var responseInfo = new
            {
                RequestId = requestId,
                StatusCode = response.StatusCode,
                ContentType = response.ContentType,
                ContentLength = response.ContentLength,
                ElapsedMs = elapsedMs,
                Headers = GetSafeHeaders(response.Headers.ToDictionary(h => h.Key, h => h.Value))
            };

            // Determine log level based on status code
            var logLevel = response.StatusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                _ => LogLevel.Information
            };

            var statusEmoji = response.StatusCode switch
            {
                >= 200 and < 300 => "✅",
                >= 300 and < 400 => "🔄",
                >= 400 and < 500 => "⚠️",
                >= 500 => "❌",
                _ => "❓"
            };

            _logger.Log(logLevel, "{Emoji} RESPONSE [{RequestId}] {StatusCode} | {ElapsedMs}ms | User: {UserId}",
                statusEmoji, requestId, response.StatusCode, elapsedMs, userId);

            // Log detailed response at Debug level
            _logger.LogDebug("RESPONSE DETAILS [{RequestId}]: {@ResponseInfo}", requestId, responseInfo);

            // Log response body at Debug level
            if (!string.IsNullOrEmpty(responseBody))
            {
                _logger.LogDebug("RESPONSE BODY [{RequestId}]: {Body}", requestId, responseBody);
            }

            // Log slow requests
            if (elapsedMs > 2000) // > 2 seconds
            {
                _logger.LogWarning("🐌 SLOW REQUEST [{RequestId}] took {ElapsedMs}ms | {Method} {Path}",
                    requestId, elapsedMs, context.Request.Method, context.Request.Path);
            }
        }

        private static string GetClientIP(HttpContext context)
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ??
                   context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                   context.Connection.RemoteIpAddress?.ToString() ??
                   "Unknown";
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            return body;
        }

        private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
        {
            var safeHeaders = new Dictionary<string, string>();
            var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key", "x-auth-token" };

            foreach (var header in headers)
            {
                if (sensitiveHeaders.Contains(header.Key.ToLowerInvariant()))
                {
                    safeHeaders[header.Key] = "***HIDDEN***";
                }
                else
                {
                    safeHeaders[header.Key] = string.Join(", ", header.Value.ToArray());
                }
            }

            return safeHeaders;
        }

        private static Dictionary<string, string> GetSafeHeaders(Dictionary<string, Microsoft.Extensions.Primitives.StringValues> headers)
        {
            var safeHeaders = new Dictionary<string, string>();
            var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key", "x-auth-token" };

            foreach (var header in headers)
            {
                if (sensitiveHeaders.Contains(header.Key.ToLowerInvariant()))
                {
                    safeHeaders[header.Key] = "***HIDDEN***";
                }
                else
                {
                    safeHeaders[header.Key] = string.Join(", ", header.Value.ToArray());
                }
            }

            return safeHeaders;
        }
    }

    // Extension method
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
