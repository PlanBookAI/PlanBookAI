using ClassroomService.Data;
using ClassroomService.Repositories.Implementations;
using ClassroomService.Repositories.Interfaces;
using ClassroomService.Services.Implementations;
using ClassroomService.Services.Interfaces;
using ClassroomService.Profiles;
using ClassroomService.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/classroomservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repository Registration
builder.Services.AddScoped<IClassesRepository, ClassesRepository>();
builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();
builder.Services.AddScoped<IStudentResultsRepository, StudentResultsRepository>();
builder.Services.AddScoped<IAnswerSheetsRepository, AnswerSheetsRepository>();

// Service Registration
builder.Services.AddScoped<IClassesService, ClassesService>();
builder.Services.AddScoped<IStudentsService, StudentsService>();
builder.Services.AddScoped<IStudentResultsService, StudentResultsService>();
builder.Services.AddScoped<IAnswerSheetsService, AnswerSheetsService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add Serilog request logging
app.UseSerilogRequestLogging();

// Add Gateway authentication middleware
app.UseGatewayAuthentication();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();
