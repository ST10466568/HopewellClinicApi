using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HopewellClinicApi.Data;
using HopewellClinicApi.Services;
using HopewellClinicApi.Middleware;
using HopewellClinicApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<HopewellDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    
    // Configure SignIn to prevent redirects
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<HopewellDbContext>();

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Configure default policy if needed
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    // Configure admin policy
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

// JWT Authentication - Set as default scheme and configure to prevent redirects
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
    {
        // Get JWT settings from configuration (try both JwtSettings and Jwt sections)
        var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? builder.Configuration["Jwt:Issuer"] ?? "HopewellClinic";
        var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? builder.Configuration["Jwt:Audience"] ?? "HopewellClinic";
        var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? builder.Configuration["Jwt:SecretKey"] ?? "YourSecretKeyHere";
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            // Map role claims from JWT token
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
        
        // Add event handlers for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Note: Logger is not available in this event handler as it's registered before app is built
                // Using Console.WriteLine as fallback
                Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Authentication failed: {context.Exception?.Message}");
                Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Exception type: {context.Exception?.GetType().Name}");
                // Don't redirect, return 401
                context.NoResult();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] JWT Challenge triggered");
                // Prevent redirects, return 401 instead
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = context.Principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Token validated for user: {userId}, Roles: {string.Join(", ", roles)}");
                return Task.CompletedTask;
            }
        };
    });

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("DebugCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://hopewell-clinic-frontend.azurewebsites.net"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24 hours
    });
    
    // Add a default policy that allows all origins (for development/testing)
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Services
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<EnhancedBookingService>();
builder.Services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<AppointmentManagementService>();
builder.Services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
builder.Services.AddScoped<AppointmentStatusService>();
builder.Services.AddScoped<IAppointmentStatusService, AppointmentStatusService>();
builder.Services.AddScoped<IAdminDoctorService, AdminDoctorService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

// Background Services
builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddHostedService<AvailabilityUpdateService>();

// Logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Get logger for startup logging
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Application starting up...");

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Apply any pending migrations
        context.Database.Migrate();
        logger.LogInformation("✅ Database migrations applied successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error applying migrations: {Message}", ex.Message);
        // Don't fail the startup, but log the error
    }
}

startupLogger.LogInformation("Application startup complete");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Enable detailed error pages in production for debugging
    app.UseDeveloperExceptionPage();
}

// Middleware pipeline - CORRECT ORDER
app.UseRouting();

// Request logging middleware - MUST be first to catch all requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var authPreview = authHeader != null && authHeader.Length > 50 ? authHeader.Substring(0, 50) + "..." : authHeader ?? "None";
        logger.LogInformation("[{Time}] Incoming Request: {Method} {Path}", DateTime.UtcNow, context.Request.Method, context.Request.Path);
        logger.LogInformation("[{Time}] Authorization Header: {Auth}", DateTime.UtcNow, authPreview);
        
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[{Time}] EXCEPTION IN REQUEST LOGGING MIDDLEWARE: {ExceptionType}", DateTime.UtcNow, ex.GetType().Name);
        throw; // Re-throw to be caught by global handler
    }
});

// Global exception handler middleware - catches ALL exceptions
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[{Time}] GLOBAL EXCEPTION HANDLER: {ExceptionType} - {Message}", 
            DateTime.UtcNow, ex.GetType().Name, ex.Message);
        
        // Ensure CORS headers are set even on error responses
        var origin = context.Request.Headers["Origin"].ToString();
        var allowedOrigins = new[] { 
            "https://hopewell-clinic-frontend.azurewebsites.net",
            "http://localhost:3000",
            "http://localhost:5173"
        };
        
        if (allowedOrigins.Contains(origin) || string.IsNullOrEmpty(origin))
        {
            context.Response.Headers.Append("Access-Control-Allow-Origin", 
                string.IsNullOrEmpty(origin) ? allowedOrigins[0] : origin);
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept");
            context.Response.Headers.Append("Access-Control-Expose-Headers", "*");
        }
        
        // Don't write response if headers already sent
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "Internal server error",
                message = ex.Message,
                type = ex.GetType().Name,
                innerException = ex.InnerException?.Message
            }));
        }
    }
});

// Add CORS headers to all responses (including errors and preflight) - MUST be before UseCors
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString().Trim();
    var allowedOrigins = new[] { 
        "https://hopewell-clinic-frontend.azurewebsites.net",
        "http://localhost:3000",
        "http://localhost:5173"
    };
    
    // Normalize origin comparison (case-insensitive, trim)
    var normalizedOrigin = origin.ToLowerInvariant();
    var isOriginAllowed = allowedOrigins.Any(ao => ao.ToLowerInvariant() == normalizedOrigin) || string.IsNullOrEmpty(origin);
    
    // Handle preflight OPTIONS requests FIRST - before checking origin
    if (context.Request.Method == "OPTIONS")
    {
        // Always respond to OPTIONS, but only set CORS headers for allowed origins
        if (isOriginAllowed)
        {
            var responseOrigin = string.IsNullOrEmpty(origin) ? allowedOrigins[0] : origin;
            
            context.Response.Headers.Append("Access-Control-Allow-Origin", responseOrigin);
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept, Origin");
            context.Response.Headers.Append("Access-Control-Expose-Headers", "*");
            context.Response.Headers.Append("Access-Control-Max-Age", "86400");
        }
        
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("");
        return; // Don't continue to next middleware
    }
    
    // For non-OPTIONS requests, set CORS headers if origin is allowed
    if (isOriginAllowed)
    {
        var responseOrigin = string.IsNullOrEmpty(origin) ? allowedOrigins[0] : origin;
        
        context.Response.Headers.Append("Access-Control-Allow-Origin", responseOrigin);
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept, Origin");
        context.Response.Headers.Append("Access-Control-Expose-Headers", "*");
        context.Response.Headers.Append("Access-Control-Max-Age", "86400");
    }
    
    await next();
});

app.UseCors("DebugCors"); // Use named policy that allows credentials
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "API is running!");

app.Run();