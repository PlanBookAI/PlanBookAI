using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GatewayService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var corsSettings = builder.Configuration.GetSection("Cors");
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        var allowedMethods = corsSettings.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        var allowedHeaders = corsSettings.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "*" };
        var allowCredentials = corsSettings.GetValue<bool>("AllowCredentials");

        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        if (allowedMethods.Contains("*"))
        {
            policy.AllowAnyMethod();
        }
        else
        {
            policy.WithMethods(allowedMethods);
        }

        if (allowedHeaders.Contains("*"))
        {
            policy.AllowAnyHeader();
        }
        else
        {
            policy.WithHeaders(allowedHeaders);
        }

        if (allowCredentials && !allowedOrigins.Contains("*"))
        {
            policy.AllowCredentials();
        }
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Memory Cache for Rate Limiting
builder.Services.AddMemoryCache();

// Add JWT Services
builder.Services.AddSingleton<GatewayService.Interfaces.IJwtService, GatewayService.Services.JwtService>();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "PlanbookAI Gateway Service", 
        Version = "v1",
        Description = "API Gateway cho hệ thống PlanbookAI - Cổng công cụ AI dành cho giáo viên THPT",
        Contact = new OpenApiContact
        {
            Name = "PlanbookAI Team",
            Email = "support@planbookai.com"
        }
    });
    
    // Add JWT authentication to Swagger
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanbookAI Gateway API V1");
        c.RoutePrefix = "swagger";
    });
}

// Use CORS
app.UseCors();

// Security Headers
var securitySettings = builder.Configuration.GetSection("Security");
if (securitySettings.GetValue<bool>("EnableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

if (securitySettings.GetValue<bool>("EnableHsts"))
{
    app.UseHsts();
}

// Add Security Headers
app.Use(async (context, next) =>
{
    if (securitySettings.GetValue<bool>("EnableXssProtection"))
    {
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    }
    
    if (securitySettings.GetValue<bool>("EnableContentTypeOptions"))
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    }
    
    if (securitySettings.GetValue<bool>("EnableFrameOptions"))
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
    }
    
    if (securitySettings.GetValue<bool>("EnableReferrerPolicy"))
    {
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    }
    
    await next();
});

// Use Error Handling Middleware (first to catch all errors)
app.UseErrorHandling();

// Use Request Logging Middleware (second to log everything)
app.UseRequestLogging();

// Use Rate Limiting Middleware (before authentication)
var rateLimitSettings = builder.Configuration.GetSection("RateLimiting");
app.UseRateLimiting(new GatewayService.Middleware.RateLimitingOptions
{
    MaxRequests = rateLimitSettings.GetValue<int>("MaxRequests", 100),
    WindowSizeInMinutes = rateLimitSettings.GetValue<int>("WindowSizeInMinutes", 1)
});

// Use JWT Middleware (before authorization)
app.UseMiddleware<GatewayService.Middleware.JwtMiddleware>();

// Map Health Checks
app.MapHealthChecks("/health");

app.UseAuthorization();
app.MapControllers();

// Map Reverse Proxy
app.MapReverseProxy();

app.Run();
