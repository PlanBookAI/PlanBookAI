namespace PlanbookAI.FileStorageService.Middleware
{
    using System.Net;
    using System.Text.Json;
    using PlanbookAI.FileStorageService.Models.DTOs;
    
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
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred");
            
            context.Response.ContentType = "application/json";
            
            var response = new PhanHoiLoi();
            
            switch (exception)
            {
                case ArgumentException:
                    response.ThongBao = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                    
                case UnauthorizedAccessException:
                    response.ThongBao = "Khong du quyen";
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                    
                case FileNotFoundException:
                    response.ThongBao = "Khong tim thay tep tin";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                    
                default:
                    response.ThongBao = "Loi he thong";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}