using Microsoft.EntityFrameworkCore;
using PlanbookAI.FileStorageService.Data;
using PlanbookAI.FileStorageService.Services;
using PlanbookAI.FileStorageService.Repositories;
using PlanbookAI.FileStorageService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Host=localhost;Database=filestorage;Username=postgres;Password=password";
builder.Services.AddDbContext<FileStorageDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<ITepTinRepository, TepTinRepository>();
builder.Services.AddScoped<IDichVuTepTin, DichVuTepTin>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GatewayAuthMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();