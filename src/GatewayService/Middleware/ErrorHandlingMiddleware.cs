using System.Net;
using System.Text.Json;

namespace GatewayService.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không mong đợi xảy ra: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case HttpRequestException httpEx:
                // Service không khả dụng
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse = new ErrorResponse
                {
                    Message = "Dịch vụ tạm thời không khả dụng. Vui lòng thử lại sau.",
                    Detail = "Service temporarily unavailable",
                    ErrorCode = "SERVICE_UNAVAILABLE",
                    Timestamp = DateTime.UtcNow
                };
                break;

            case TaskCanceledException tcEx when tcEx.InnerException is TimeoutException:
                // Timeout
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse = new ErrorResponse
                {
                    Message = "Yêu cầu đã hết thời gian chờ. Vui lòng thử lại.",
                    Detail = "Request timeout",
                    ErrorCode = "REQUEST_TIMEOUT",
                    Timestamp = DateTime.UtcNow
                };
                break;

            case UnauthorizedAccessException:
                // Không có quyền truy cập
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse
                {
                    Message = "Bạn không có quyền truy cập tài nguyên này.",
                    Detail = "Unauthorized access",
                    ErrorCode = "UNAUTHORIZED",
                    Timestamp = DateTime.UtcNow
                };
                break;

            case ArgumentException argEx:
                // Dữ liệu đầu vào không hợp lệ
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Detail = argEx.Message,
                    ErrorCode = "INVALID_INPUT",
                    Timestamp = DateTime.UtcNow
                };
                break;

            default:
                // Lỗi server nội bộ
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    Message = "Đã xảy ra lỗi nội bộ. Vui lòng liên hệ quản trị viên.",
                    Detail = "Internal server error",
                    ErrorCode = "INTERNAL_ERROR",
                    Timestamp = DateTime.UtcNow
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? TraceId { get; set; }
}

// Extension method để dễ dàng sử dụng
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
