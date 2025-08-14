using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;
using UserService.Repositories;
using UserService.Middleware;
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
    
    // Thêm JWT authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

// Sử dụng HeaderAuthenticationMiddleware để đọc headers từ Gateway
app.UseHeaderAuthentication();

// Thêm authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Thiết lập Database Connection - sử dụng database thực
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    // Chỉ đảm bảo database connection, không tạo test data
    await context.Database.CanConnectAsync();
}

app.Run();

