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


