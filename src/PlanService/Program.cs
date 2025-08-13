using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PlanService.Data;
using PlanService.Repositories;
using PlanService.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Thêm Entity Framework DbContext
builder.Services.AddDbContext<PlanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm các Repository
builder.Services.AddScoped<IGiaoAnRepository, GiaoAnRepository>();
builder.Services.AddScoped<IMauGiaoAnRepository, MauGiaoAnRepository>();

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
app.UseAuthorization();
app.MapControllers();

// Thiết lập Migration và tạo dữ liệu test
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlanDbContext>();
    context.Database.EnsureCreated();
    await TaoDuLieuTest(context);
}

app.Run();

// Method tạo dữ liệu test
async Task TaoDuLieuTest(PlanDbContext context)
{
    // Kiểm tra xem đã có dữ liệu test chưa
    if (await context.GiaoAns.AnyAsync()) return;

    // Tạo test lesson plans
    var testGiaoAns = new List<GiaoAn>
    {
        new GiaoAn
        {
            Id = Guid.NewGuid(),
            TieuDe = "Bài 1: Nguyên tử và phân tử",
            MucTieu = "Học sinh hiểu được cấu tạo nguyên tử và phân tử",
            NoiDung = "Nội dung chi tiết về nguyên tử và phân tử...",
            MonHoc = "HoaHoc", // ← STRING thay vì enum
            Lop = 10,
            GiaoVienId = Guid.NewGuid(),
            TrangThai = "DRAFT", // ← STRING thay vì enum
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        },
        new GiaoAn
        {
            Id = Guid.NewGuid(),
            TieuDe = "Bài 2: Liên kết hóa học",
            MucTieu = "Học sinh nắm được các loại liên kết hóa học",
            NoiDung = "Nội dung chi tiết về liên kết hóa học...",
            MonHoc = "HoaHoc", // ← STRING thay vì enum
            Lop = 10,
            GiaoVienId = Guid.NewGuid(),
            TrangThai = "COMPLETED", // ← STRING thay vì enum
            TaoLuc = DateTime.UtcNow,
            CapNhatLuc = DateTime.UtcNow
        }
    };

    await context.GiaoAns.AddRangeAsync(testGiaoAns);
    await context.SaveChangesAsync();
}
