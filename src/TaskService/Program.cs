using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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

// Thiết lập Migration - tạo database từ DbContext
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    context.Database.EnsureCreated();

    // Tạo dữ liệu test
    await TaoDuLieuTest(context);
}

app.Run();

// Method tạo dữ liệu test
async Task TaoDuLieuTest(TaskDbContext context)
{
    // Kiểm tra xem đã có dữ liệu test chưa
    if (await context.CauHois.AnyAsync()) return;

    // Tạo test questions
    var testQuestions = new List<CauHoi>
    {
        new CauHoi
        {
            id = Guid.NewGuid().ToString(),
            noiDung = "Nguyên tử có cấu trúc như thế nào?",
            monHoc = "HoaHoc",
            doKho = "TrungBinh",
            dapAnDung = "A"
        },
        new CauHoi
        {
            id = Guid.NewGuid().ToString(),
            noiDung = "Công thức hóa học của nước là gì?",
            monHoc = "HoaHoc",
            doKho = "De",
            dapAnDung = "B"
        }
    };

    await context.CauHois.AddRangeAsync(testQuestions);
    await context.SaveChangesAsync();
}
