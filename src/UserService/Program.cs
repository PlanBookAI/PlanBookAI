using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;
using UserService.Repositories;
// BỎ dòng: using UserService.Models.Entities; để tránh conflict

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container
builder.Services.AddControllers();

// Thêm Entity Framework DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm các dịch vụ nghiệp vụ
builder.Services.AddScoped<IDichVuNguoiDung, DichVuNguoiDung>();

// Thêm các Repository
builder.Services.AddScoped<IHoSoNguoiDungRepository, HoSoNguoiDungRepository>();

// Thêm Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "API documentation for User Management Service"
    });
});

var app = builder.Build();

// Cấu hình pipeline xử lý HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Thiết lập Migration - tạo database từ DbContext
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    context.Database.EnsureCreated();

    // Tạo dữ liệu test
    await TaoDuLieuTest(context);
}

app.Run();

// Method tạo dữ liệu test
async Task TaoDuLieuTest(UserDbContext context)
{
    // Kiểm tra xem đã có dữ liệu test chưa
    if (await context.HoSoNguoiDungs.AnyAsync()) return;

    // Tạo test profiles với fully qualified name để tránh conflict
    var testProfiles = new List<UserService.Models.Entities.HoSoNguoiDung>
    {
        new UserService.Models.Entities.HoSoNguoiDung
        {
            UserId = Guid.NewGuid(),
            HoTen = "Admin Test User",
            SoDienThoai = "0123456789",
            NgaySinh = new DateTime(1990, 1, 1),
            DiaChi = "Hà Nội, Việt Nam",
            AnhDaiDienUrl = "https://example.com/admin-avatar.jpg",
            MoTaBanThan = "Admin user profile for testing",
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        },
        new UserService.Models.Entities.HoSoNguoiDung
        {
            UserId = Guid.NewGuid(),
            HoTen = "Teacher Test User",
            SoDienThoai = "0987654321",
            NgaySinh = new DateTime(1985, 5, 15),
            DiaChi = "TP. Hồ Chí Minh, Việt Nam",
            AnhDaiDienUrl = "https://example.com/teacher-avatar.jpg",
            MoTaBanThan = "Teacher user profile for testing",
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        }
    };

    await context.HoSoNguoiDungs.AddRangeAsync(testProfiles);
    await context.SaveChangesAsync();
}