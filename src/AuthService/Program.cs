using AuthService.Data;
using AuthService.Models.Entities;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container
builder.Services.AddControllers();

// Thêm Entity Framework DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Thêm các dịch vụ nghiệp vụ
builder.Services.AddScoped<IDichVuXacThuc, DichVuXacThuc>();

// Thêm các Repository
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IVaiTroRepository, VaiTroRepository>();

// Thêm Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "API documentation for Authentication Service"
    });
});

var app = builder.Build();

// Cấu hình pipeline xử lý HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Thiết lập Migration - tạo database từ DbContext
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    context.Database.EnsureCreated(); // Tạo database và tables

    // Tạo dữ liệu test
    await TaoDuLieuTest(context);
}

app.Run();

// Method tạo dữ liệu test
async Task TaoDuLieuTest(AuthDbContext context)
{
    // Kiểm tra xem đã có dữ liệu test chưa
    if (await context.NguoiDungs.AnyAsync()) return;

    // Tạo test users
    var testUsers = new List<NguoiDung>
    {
        new NguoiDung
        {
            Id = Guid.NewGuid(),
            Email = "admin@planbookai.dev",
            HoTen = "Admin Test",
            MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("admin123"),
            VaiTroId = 1, // ADMIN
            LaHoatDong = true,
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        },
        new NguoiDung
        {
            Id = Guid.NewGuid(),
            Email = "teacher@planbookai.dev",
            HoTen = "Giáo Viên Test",
            MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("teacher123"),
            VaiTroId = 4, // TEACHER
            LaHoatDong = true,
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        }
    };

    await context.NguoiDungs.AddRangeAsync(testUsers);
    await context.SaveChangesAsync();
}
