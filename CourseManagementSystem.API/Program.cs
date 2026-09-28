using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using CourseManagementSystem.API.Middleware;
using CourseManagementSystem.Application.Services;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Infrastructure.Repositories;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// JWT CONFIGURATION
// =====================================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// =====================================================
// CONTROLLERS
// =====================================================

builder.Services.AddControllers();

// =====================================================
// DATABASE
// =====================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// =====================================================
// REPOSITORIES
// =====================================================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// =====================================================
// SERVICES
// =====================================================

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// =====================================================
// JWT AUTHENTICATION
// =====================================================

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });

// =====================================================
// AUTHORIZATION
// =====================================================

builder.Services.AddAuthorization();

// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});

// =====================================================
// BUILD APPLICATION
// =====================================================

var app = builder.Build();

// =====================================================
// GLOBAL EXCEPTION MIDDLEWARE
// =====================================================

app.UseMiddleware<GlobalExceptionMiddleware>();

// =====================================================
// SWAGGER
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();

// =====================================================
// AUTHENTICATION
// =====================================================

app.UseAuthentication();

// =====================================================
// AUTHORIZATION
// =====================================================

app.UseAuthorization();

// =====================================================
// CONTROLLERS
// =====================================================

app.MapControllers();

// =====================================================
// RUN APPLICATION
// =====================================================

app.Run();