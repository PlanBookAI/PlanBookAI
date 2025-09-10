using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Repositories;
using PlanService.Models.Entities;
using PlanService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Thêm Entity Framework DbContext
builder.Services.AddDbContext<PlanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm các Repository
builder.Services.AddScoped<IChuDeRepository, ChuDeRepository>();
builder.Services.AddScoped<IGiaoAnRepository, GiaoAnRepository>();
builder.Services.AddScoped<IMauGiaoAnRepository, MauGiaoAnRepository>();

// Thêm các Services
builder.Services.AddScoped<IChuDeService, ChuDeService>();
builder.Services.AddScoped<IGiaoAnService, GiaoAnService>();
builder.Services.AddScoped<IMauGiaoAnService, MauGiaoAnService>();
// builder.Services.AddScoped<PlanService.Services.Export.IXuatGiaoAnWordService, PlanService.Services.Export.XuatGiaoAnWordService>();
// builder.Services.AddScoped<PlanService.Services.Export.IXuatGiaoAnPdfService, PlanService.Services.Export.XuatGiaoAnPdfService>();

// Cấu hình JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt["SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

    });

// Authorization: Chỉ có giáo viên mới được phép tạo/quản lý giáo án
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly", p => p.RequireRole("TEACHER"));
}); 

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "PlanService API", 
        Version = "v1",
        Description = "API documentation for Lesson Plan Service"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanService API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Thiết lập Database Connection - sử dụng database thực
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlanDbContext>();
    try
    {
        // Tạo database và schema nếu chưa tồn tại
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("PlanService: Database schema created successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"PlanService: Database setup error: {ex.Message}");
    }
}

app.Run();


