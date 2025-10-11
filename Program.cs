using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.Services;
using HopewellClinicApi.Middleware;
using HopewellClinicApi.Attributes;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework with retry policy
builder.Services.AddDbContext<HopewellDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    })
    .ConfigureWarnings(warnings => 
        warnings.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
});

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<HopewellDbContext>()
.AddDefaultTokenProviders();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<IAdminDoctorService, AdminDoctorService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ClockSkew = TimeSpan.Zero
        };
        
        // Remove all events to eliminate potential issues
    });

// Add Authorization
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://hopewell-clinic-frontend.azurewebsites.net",
                "https://hopewellapi-azcvcferesfpgjgm.southafricanorth-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Add a more permissive policy for debugging and production
    options.AddPolicy("DebugCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    
    // Production CORS policy
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://hopewell-clinic-frontend.azurewebsites.net",
                "https://hopewellapi-azcvcferesfpgjgm.southafricanorth-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS first - use most permissive policy temporarily to fix CORS issue
app.UseCors("DebugCors");

// Add explicit CORS headers as fallback - ensure they're always set
app.Use(async (context, next) =>
{
    // Always set CORS headers first
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
    
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }
    
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // Ensure CORS headers are still set even on exceptions
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
        
        // Log the exception for debugging
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception in request pipeline");
        
        throw; // Re-throw to let the error handling middleware handle it
    }
});

// Add Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add routing
app.UseRouting();

// Add a simple health check endpoint
app.MapGet("/health", () => "API is running!");

// Map controllers
app.MapControllers();

// Database migration and seeding
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();
        db.Database.Migrate();
        
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        await DbInitializer.SeedUsers(userManager, roleManager);
        
        // Seed patients
        await DbInitializer.SeedPatients(db, userManager);
        
        // Seed staff
        await DbInitializer.SeedStaff(db, userManager);
        
        // Seed services
        await DbInitializer.SeedServices(db);
        
        // Seed doctor schedules - temporarily disabled due to foreign key constraint
        // await DoctorScheduleSeeder.SeedDoctorSchedulesAsync(db);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database");
}

// For Azure App Service, don't specify URL - let the hosting environment handle it
// For local development, use port 5004
if (app.Environment.IsDevelopment())
{
    app.Run("http://localhost:5002");
}
else
{
    app.Run();
}