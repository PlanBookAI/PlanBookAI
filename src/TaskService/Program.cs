using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using TaskService.Data;
using TaskService.Middleware;
using TaskService.Models.Entities;
using TaskService.Repositories;

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

// Thêm Entity Framework DbContext
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add model validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .Select(x => new { 
                Field = x.Key, 
                Errors = x.Value?.Errors.Select(e => e.ErrorMessage ?? "Unknown error") ?? Enumerable.Empty<string>() 
            })
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

// Add repositories - sử dụng database thực
builder.Services.AddScoped<ICauHoiRepository, CauHoiRepository>();
builder.Services.AddScoped<IDeThiRepository, DeThiRepository>();

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

// Add header authentication middleware
app.UseMiddleware<HeaderAuthenticationMiddleware>();

app.UseRouting(); // Đảm bảo UseRouting() được gọi

app.UseAuthorization();

app.MapControllers();

// Thiết lập Database Connection - không tạo mock data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    // Chỉ đảm bảo database connection, không tạo mock data
    await context.Database.CanConnectAsync();
}

app.Run();
