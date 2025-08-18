using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;
using UserService.Repositories;
using FluentValidation;
using UserService.Models.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// FluentValidation DI (manual validation, không bật auto-validation)
builder.Services.AddValidatorsFromAssemblyContaining<YeuCauCapNhatHoSoValidator>(ServiceLifetime.Transient);

// Add Repositories
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IVaiTroRepository, VaiTroRepository>();
builder.Services.AddScoped<ILichSuDangNhapRepository, LichSuDangNhapRepository>();

// Add Services
builder.Services.AddScoped<IDichVuNguoiDung, DichVuNguoiDung>();
builder.Services.AddScoped<IDichVuLichSuDangNhap, DichVuLichSuDangNhap>();

// Add CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Không cần authentication middleware - Gateway sẽ xử lý
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "UserService is running!");

app.Run();
