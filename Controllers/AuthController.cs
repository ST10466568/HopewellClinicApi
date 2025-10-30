using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using System.Security.Cryptography;
using HopewellClinicApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HopewellClinicApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HopewellDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private static readonly Dictionary<string, UserSession> _activeSessions = new();

        public AuthController(
            UserManager<ApplicationUser> userManager, 
            HopewellDbContext context, 
            JwtService jwtService,
            IPasswordResetService passwordResetService,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _context = context;
            _jwtService = jwtService;
            _passwordResetService = passwordResetService;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterPatientDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return BadRequest(new { error = "User with this email already exists." });
            }

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, "patient");

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PatientNumber = $"PAT{DateTime.UtcNow.Ticks}", // Simple unique number
                DateOfBirth = request.DateOfBirth,
                Address = request.Address
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Generate JWT token and get user roles
            var jwtToken = await _jwtService.GenerateToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = "User registered successfully.",
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = "patient",
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    roles = userRoles
                }
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { error = "Invalid credentials." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { error = "Invalid credentials." });
            }

            // Generate JWT token
            var jwtToken = await _jwtService.GenerateToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = userRoles.FirstOrDefault() ?? "user",
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    roles = userRoles
                }
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                if (_activeSessions.ContainsKey(token))
                {
                    _activeSessions.Remove(token);
                }
            }

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var session = GetSessionFromHeader(Request, out var errorResponse);
            if (session == null)
            {
                return errorResponse!;
            }

            var user = await _userManager.FindByIdAsync(session.UserId.ToString());
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // If user is a patient, include patient information
            if (session.Roles.Contains("patient"))
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    roles = session.Roles,
                    patientId = patient?.Id,
                    patientNumber = patient?.PatientNumber
                });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                roles = session.Roles
            });
        }

        private static string GenerateSessionToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static UserSession? GetSessionFromToken(string token)
        {
            if (!_activeSessions.TryGetValue(token, out var session))
            {
                return null;
            }

            if (session.ExpiresAt < DateTime.UtcNow)
            {
                _activeSessions.Remove(token);
                return null;
            }

            return session;
        }

        private static UserSession? GetSessionFromHeader(HttpRequest request, out IActionResult? errorResult)
        {
            errorResult = null;
            var value = request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(value) || !value.StartsWith("Bearer "))
            {
                errorResult = new UnauthorizedObjectResult(new { error = "No valid token provided" });
                return null;
            }

            var token = value.Substring("Bearer ".Length);
            return GetSessionFromToken(token);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new ForgotPasswordResponse
                    {
                        Success = false,
                        Error = "Email is required"
                    });
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // For security reasons, don't reveal if the email exists or not
                    return Ok(new ForgotPasswordResponse
                    {
                        Success = true,
                        Message = "If an account with that email exists, password reset instructions have been sent."
                    });
                }

                if (!user.IsActive)
                {
                    return Ok(new ForgotPasswordResponse
                    {
                        Success = true,
                        Message = "If an account with that email exists, password reset instructions have been sent."
                    });
                }

                // Generate reset token
                var resetToken = await _passwordResetService.GenerateResetTokenAsync(user.Id);

                // Send password reset email
                var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={resetToken}";
                var emailSubject = "Reset Your Hopewell Clinic Password";
                var emailBody = $@"
Dear {user.FirstName} {user.LastName},

You requested to reset your password for your Hopewell Clinic account.

Click the link below to reset your password:
{resetLink}

This link will expire in 1 hour for security reasons.

If you didn't request this password reset, please ignore this email.

Best regards,
Hopewell Clinic Team
";

                var emailResult = await _emailService.SendEmailAsync(user.Email!, emailSubject, emailBody, emailBody);
                
                if (!emailResult.Success)
                {
                    _logger.LogError("Failed to send password reset email to {Email}: {Error}", user.Email, emailResult.Error);
                    return StatusCode(500, new ForgotPasswordResponse
                    {
                        Success = false,
                        Error = "Failed to send password reset email. Please try again later."
                    });
                }

                _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                
                return Ok(new ForgotPasswordResponse
                {
                    Success = true,
                    Message = "If an account with that email exists, password reset instructions have been sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request for {Email}", request.Email);
                return StatusCode(500, new ForgotPasswordResponse
                {
                    Success = false,
                    Error = "An error occurred while processing your request. Please try again later."
                });
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new ResetPasswordResponse
                    {
                        Success = false,
                        Error = "Token and new password are required"
                    });
                }

                // Validate the reset token
                if (!await _passwordResetService.ValidateResetTokenAsync(request.Token))
                {
                    return BadRequest(new ResetPasswordResponse
                    {
                        Success = false,
                        Error = "Invalid or expired reset token"
                    });
                }

                // Get user by token
                var user = await _passwordResetService.GetUserByResetTokenAsync(request.Token);
                if (user == null)
                {
                    return BadRequest(new ResetPasswordResponse
                    {
                        Success = false,
                        Error = "Invalid or expired reset token"
                    });
                }

                // Validate password strength
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var validationResult = await passwordValidator.ValidateAsync(_userManager, user, request.NewPassword);
                
                if (!validationResult.Succeeded)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.Description));
                    return BadRequest(new ResetPasswordResponse
                    {
                        Success = false,
                        Error = $"Password does not meet requirements: {errors}"
                    });
                }

                // Reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest(new ResetPasswordResponse
                    {
                        Success = false,
                        Error = $"Failed to reset password: {errors}"
                    });
                }

                // Mark token as used
                await _passwordResetService.MarkTokenAsUsedAsync(request.Token);

                // Update user's UpdatedAt timestamp
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password successfully reset for user {Email}", user.Email);

                return Ok(new ResetPasswordResponse
                {
                    Success = true,
                    Message = "Your password has been successfully reset. You can now log in with your new password."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing reset password request");
                return StatusCode(500, new ResetPasswordResponse
                {
                    Success = false,
                    Error = "An error occurred while processing your request. Please try again later."
                });
            }
        }

        [HttpPut("profile/{userId}")]
        [Authorize]
        public async Task<ActionResult<ProfileUpdateResponse>> UpdateProfile(Guid userId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                // Get authenticated user ID from JWT token
                var authenticatedUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                                User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(authenticatedUserIdClaim) || 
                    !Guid.TryParse(authenticatedUserIdClaim, out var authenticatedUserId))
                {
                    return Unauthorized(new ProfileUpdateResponse
                    {
                        Success = false,
                        Error = "Unauthorized",
                        Message = "Invalid authentication token"
                    });
                }

                // Verify user can only update their own profile
                if (authenticatedUserId != userId)
                {
                    _logger.LogWarning("User {AuthenticatedUserId} attempted to update profile of user {TargetUserId}", 
                        authenticatedUserId, userId);
                    return StatusCode(403, new ProfileUpdateResponse
                    {
                        Success = false,
                        Error = "Unauthorized",
                        Message = "You can only update your own profile"
                    });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.First().ErrorMessage ?? "Invalid value"
                        );

                    return BadRequest(new ProfileUpdateResponse
                    {
                        Success = false,
                        Error = "Validation Error",
                        Message = "Invalid input data",
                        Errors = errors
                    });
                }

                // Get user from database
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new ProfileUpdateResponse
                    {
                        Success = false,
                        Error = "User Not Found",
                        Message = $"User with ID {userId} not found"
                    });
                }

                // Check if email is being changed and validate uniqueness
                if (!string.IsNullOrEmpty(request.Email) && 
                    !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _userManager.FindByEmailAsync(request.Email);
                    if (emailExists != null && emailExists.Id != userId)
                    {
                        return BadRequest(new ProfileUpdateResponse
                        {
                            Success = false,
                            Error = "Email Exists",
                            Message = "Email address is already in use by another account",
                            Errors = new Dictionary<string, string> { { "email", "Email address is already in use" } }
                        });
                    }
                }

                // Update user fields
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.Email))
                {
                    user.Email = request.Email;
                    user.UserName = request.Email; // Update username to match email
                }

                if (request.Phone != null)
                {
                    user.PhoneNumber = request.Phone;
                }

                // Serialize address to JSON string if provided
                if (request.Address != null)
                {
                    var addressJson = JsonSerializer.Serialize(request.Address);
                    user.Address = addressJson;
                }

                // Serialize emergency contact to JSON string if provided
                if (request.EmergencyContact != null)
                {
                    var emergencyContactJson = JsonSerializer.Serialize(request.EmergencyContact);
                    user.EmergencyContact = emergencyContactJson;
                    
                    // Also update EmergencyPhone if provided
                    if (!string.IsNullOrEmpty(request.EmergencyContact.Phone))
                    {
                        user.EmergencyPhone = request.EmergencyContact.Phone;
                    }
                }

                user.UpdatedAt = DateTime.UtcNow;

                // Update user in database
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.ToDictionary(
                        e => e.Code,
                        e => e.Description
                    );

                    return BadRequest(new ProfileUpdateResponse
                    {
                        Success = false,
                        Error = "Update Failed",
                        Message = "Failed to update user profile",
                        Errors = errors
                    });
                }

                // If user is a patient, also update Patient record
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient != null)
                {
                    if (request.Address != null)
                    {
                        // Store address as simple string for Patient table compatibility
                        var addressParts = new List<string>();
                        if (!string.IsNullOrEmpty(request.Address.Street)) addressParts.Add(request.Address.Street);
                        if (!string.IsNullOrEmpty(request.Address.City)) addressParts.Add(request.Address.City);
                        if (!string.IsNullOrEmpty(request.Address.State)) addressParts.Add(request.Address.State);
                        if (!string.IsNullOrEmpty(request.Address.ZipCode)) addressParts.Add(request.Address.ZipCode);
                        if (!string.IsNullOrEmpty(request.Address.Country)) addressParts.Add(request.Address.Country);
                        
                        patient.Address = addressParts.Any() ? string.Join(", ", addressParts) : null;
                    }

                    if (request.EmergencyContact != null)
                    {
                        if (!string.IsNullOrEmpty(request.EmergencyContact.Name))
                        {
                            patient.EmergencyContactName = request.EmergencyContact.Name;
                        }
                        if (!string.IsNullOrEmpty(request.EmergencyContact.Phone))
                        {
                            patient.EmergencyContactPhone = request.EmergencyContact.Phone;
                        }
                    }

                    patient.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Parse address and emergency contact for response
                AddressDto? addressDto = null;
                if (!string.IsNullOrEmpty(user.Address))
                {
                    try
                    {
                        addressDto = JsonSerializer.Deserialize<AddressDto>(user.Address);
                    }
                    catch
                    {
                        // If parsing fails, try to extract from simple string format
                        addressDto = new AddressDto { Street = user.Address };
                    }
                }

                EmergencyContactDto? emergencyContactDto = null;
                if (!string.IsNullOrEmpty(user.EmergencyContact))
                {
                    try
                    {
                        emergencyContactDto = JsonSerializer.Deserialize<EmergencyContactDto>(user.EmergencyContact);
                        if (emergencyContactDto != null && string.IsNullOrEmpty(emergencyContactDto.Phone))
                        {
                            emergencyContactDto.Phone = user.EmergencyPhone;
                        }
                    }
                    catch
                    {
                        // If parsing fails, use simple string format
                        emergencyContactDto = new EmergencyContactDto 
                        { 
                            Name = user.EmergencyContact,
                            Phone = user.EmergencyPhone
                        };
                    }
                }

                _logger.LogInformation("Profile updated successfully for user {UserId}", userId);

                return Ok(new ProfileUpdateResponse
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    Data = new ProfileDataDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        Phone = user.PhoneNumber,
                        Address = addressDto,
                        EmergencyContact = emergencyContactDto,
                        UpdatedAt = user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return StatusCode(500, new ProfileUpdateResponse
                {
                    Success = false,
                    Error = "Internal Server Error",
                    Message = "An error occurred while updating your profile. Please try again later."
                });
            }
        }
    }

    public class UserSession
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

