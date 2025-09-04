using ExamService.Data;
using ExamService.Interfaces;
using ExamService.Middleware;
using ExamService.Profiles;
using ExamService.Profiles;
using ExamService.Repositories;
using ExamService.Services;
using ExamService.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
builder.Services.AddDbContext<ExamDbContext>(options =>
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
        Title = "ExamService API",
        Version = "v1",
        Description = "API quản lý câu hỏi, đề thi và các chức năng liên quan cho PlanbookAI."
    });
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

// Add repositories - sử dụng database thực
builder.Services.AddScoped<ICauHoiRepository, CauHoiRepository>();  
builder.Services.AddScoped<IDeThiRepository, DeThiRepository>();
builder.Services.AddScoped<IMauDeThiRepository, MauDeThiRepository>();

// Add services (Business Logic Layer)
builder.Services.AddScoped<ICauHoiService, CauHoiService>();
builder.Services.AddScoped<IDeThiService, DeThiService>();
builder.Services.AddScoped<ILuaChonService, LuaChonService>();
builder.Services.AddScoped<IDeThiCauHoiService, DeThiCauHoiService>();
builder.Services.AddScoped<ITaoDeThiService, TaoDeThiService>();
builder.Services.AddScoped<IMauDeThiService, MauDeThiService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IEventLogger, EventLoggerService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MauDeThiRequestDTOValidator>();

// === Cấu hình Health Checks ===
builder.Services.AddHealthChecks()
    // Thêm check kết nối tới PostgreSQL database
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "database_postgres",
        tags: new[] { "database", "postgres", "ready" })
    // Thêm một check tùy chỉnh đơn giản để kiểm tra bộ nhớ
    .AddCheck<MemoryHealthCheck>(
        "memory_check",
        tags: new[] { "system", "live" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExamService API V1");
    });
}

// Middleware order quan trọng
app.UseHttpsRedirection();

// === Cấu hình Middleware cho Metrics và Health Checks ===
// Middleware này phải được đặt trước UseAuthorization và MapControllers

// Expose endpoint /metrics cho Prometheus
app.UseMetricServer();
app.UseHttpMetrics();

// Add custom logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Add header authentication middleware
app.UseMiddleware<HeaderAuthenticationMiddleware>();

app.UseRouting(); // Đảm bảo UseRouting() được gọi

app.UseAuthorization();

// Map các endpoint health checks
app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
{
    // Tùy chỉnh response trả về
    ResponseWriter = WriteHealthCheckResponse,
    // Chạy tất cả các check (cả 'ready' và 'live')
    Predicate = _ => true
});

app.MapHealthChecks("/api/v1/health/ready", new HealthCheckOptions
{
    // Chỉ chạy các check được đánh tag 'ready' (ví dụ: database)
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/api/v1/health/live", new HealthCheckOptions
{
    // Chỉ chạy các check được đánh tag 'live' (ví dụ: memory)
    // Liveness probes nên cực kỳ nhanh và không phụ thuộc vào dịch vụ bên ngoài
    Predicate = (check) => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthCheckResponse
});

// Map endpoint /metrics riêng (Prometheus library tự động làm điều này, nhưng để rõ ràng hơn)
app.MapMetrics();

app.MapControllers();

// Thiết lập Database Connection - không tạo mock data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExamDbContext>();
    // Chỉ đảm bảo database connection, không tạo mock data
    await context.Database.CanConnectAsync();
}

app.Run();

// === Hàm helper để tùy chỉnh response của Health Check ===
static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    var options = new JsonSerializerOptions { WriteIndented = true };

    var json = JsonSerializer.Serialize(new
    {
        Status = report.Status.ToString(),
        TotalDuration = report.TotalDuration.TotalMilliseconds,
        Entries = report.Entries.Select(e => new
        {
            Name = e.Key,
            Status = e.Value.Status.ToString(),
            Description = e.Value.Description,
            Duration = e.Value.Duration.TotalMilliseconds,
            Tags = e.Value.Tags
        })
    }, options);

    return context.Response.WriteAsync(json);
}