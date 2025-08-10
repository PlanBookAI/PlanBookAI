using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using TaskService.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using TaskService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Đảm bảo model validation hoạt động
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false;
})
.AddJsonOptions(options =>
{
    // Sửa JSON options để tránh conflict
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.WriteIndented = true;
    // Bỏ PropertyNameCaseInsensitive để tránh conflict
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
});

// Add model validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
            .ToList();

        return new BadRequestObjectResult(new { Message = "Validation failed", Errors = errors });
    };
});

// Bỏ AddMvcCore() vì conflict với AddControllers()

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TaskService API", 
        Version = "v1",
        Description = "API documentation for Task Management Service"
    });
});

// Add repositories
builder.Services.AddScoped<ICauHoiRepository, MockCauHoiRepository>();
builder.Services.AddScoped<IDeThiRepository, MockDeThiRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskService API V1");
    });
}

// Middleware order quan trọng
app.UseHttpsRedirection();

// Add custom logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting(); // Đảm bảo UseRouting() được gọi

app.UseAuthorization();

app.MapControllers();

app.Run();
