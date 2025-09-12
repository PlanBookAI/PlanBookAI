using System.Net;
using System.Text.Json;

namespace ExamService.Middleware;

public class LoiToanCucMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoiToanCucMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public LoiToanCucMiddleware(
        RequestDelegate next,
        ILogger<LoiToanCucMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Đã xảy ra lỗi: {Message}", ex.Message);
            await XuLyNgoaiLeAsync(context, ex);
        }
    }

    private async Task XuLyNgoaiLeAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var phanHoi = new
        {
            MaLoi = "LOI_HE_THONG",
            ThongBao = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.",
            ChiTiet = _env.IsDevelopment() ? exception.ToString() : "Vui lòng liên hệ quản trị viên."
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(phanHoi, options);
        await context.Response.WriteAsync(json);
    }
}

// Extension method để dễ dàng thêm middleware vào pipeline
public static class LoiToanCucMiddlewareExtensions
{
    public static IApplicationBuilder UseLoiToanCuc(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoiToanCucMiddleware>();
    }
}
