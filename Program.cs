using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
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
})
.AddEntityFrameworkStores<HopewellDbContext>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "HopewellClinic",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "HopewellClinic",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "YourSecretKeyHere"))
        };
        
        // Add event handlers for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name}");
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
                "https://hopewell-clinic-frontend.azurewebsites.net",
                "https://hopewell-clinic-frontend.azurewebsites.net/"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24 hours
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

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();
    try
    {
        // Apply any pending migrations
        context.Database.Migrate();
        Console.WriteLine("✅ Database migrations applied successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error applying migrations: {ex.Message}");
        // Don't fail the startup, but log the error
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline - CORRECT ORDER
app.UseRouting();
app.UseCors("DebugCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "API is running!");

app.Run();