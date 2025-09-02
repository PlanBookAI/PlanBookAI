
using NotificationService.Repositories;
using NotificationService.Services;
using NotificationService.Workers;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container.
builder.Services.AddControllers();

// Đăng ký các Repositories và Services
// Sử dụng AddScoped để đảm bảo mỗi request HTTP sẽ có một instance riêng.
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<EmailQueueRepository>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<EmailService>();

// Đăng ký EmailWorker như một Hosted Service.
// Điều này giúp worker chạy ở chế độ nền khi ứng dụng khởi động.
builder.Services.AddHostedService<EmailWorker>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Cấu hình HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
