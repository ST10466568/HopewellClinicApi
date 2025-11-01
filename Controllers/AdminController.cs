using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.Logging;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Models;
using HopewellClinicApi.Services;
using System.Text.Json;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAdminDoctorService _adminDoctorService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(HopewellDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IAdminDoctorService adminDoctorService, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminDoctorService = adminDoctorService;
            _logger = logger;
        }

        /// <summary>
        /// Simple test endpoint to verify route discovery - TOP OF CONTROLLER
        /// </summary>
        [HttpGet("test-route-discovery")]
        [AllowAnonymous]
        public ActionResult TestRouteDiscovery()
        {
            return Ok(new { message = "Route discovery working!", timestamp = DateTime.Now });
        }

        [HttpPost("create-staff")]
        public async Task<ActionResult<UserCreationResponse>> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "User with this email already exists" 
                    });
                }

                // Create new user
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle date of birth
                if (!string.IsNullOrWhiteSpace(request.DateOfBirth))
                {
                    if (DateTime.TryParseExact(request.DateOfBirth, "yyyy-MM-dd", 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        user.DateOfBirth = dateOfBirth;
                    }
                    else
                    {
                        return BadRequest(new UserCreationResponse 
                        { 
                            Success = false, 
                            Error = "Invalid date of birth format. Use YYYY-MM-DD" 
                        });
                    }
                }

                // Handle address
                user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address;

                // Handle emergency contact
                user.EmergencyContact = string.IsNullOrWhiteSpace(request.EmergencyContact) ? null : request.EmergencyContact;
                user.EmergencyPhone = string.IsNullOrWhiteSpace(request.EmergencyPhone) ? null : request.EmergencyPhone;

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "Failed to create user", 
                        Data = result.Errors 
                    });
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, request.Role);

                // Create staff record
                var staff = new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    StaffNumber = $"STF{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();

                return Ok(new UserCreationResponse 
                { 
                    Success = true, 
                    Message = "Staff user created successfully", 
                    Data = new { 
                        staffId = staff.Id, 
                        userId = user.Id,
                        user = new {
                            id = user.Id,
                            email = user.Email,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            phone = user.PhoneNumber,
                            dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                            address = user.Address,
                            emergencyContact = user.EmergencyContact,
                            emergencyPhone = user.EmergencyPhone,
                            role = request.Role,
                            isActive = user.IsActive,
                            createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        }
                    } 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserCreationResponse 
                { 
                    Success = false, 
                    Error = "Internal server error: " + ex.Message 
                });
            }
        }

        [HttpPost("patients")]
        public async Task<ActionResult<UserCreationResponse>> CreatePatient([FromBody] CreatePatientRequest request)
        {
            try
            {
                // Validate date of birth format
                if (!DateTime.TryParse(request.DateOfBirth, out var dateOfBirth))
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "Invalid date of birth format. Use YYYY-MM-DD" 
                    });
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "User with this email already exists" 
                    });
                }

                // Create new user
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "Failed to create user", 
                        Data = result.Errors 
                    });
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, request.Role);

                // Create patient record
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PatientNumber = $"PAT{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}",
                    DateOfBirth = dateOfBirth,
                    Address = request.Address,
                    EmergencyContactName = request.EmergencyContact,
                    EmergencyContactPhone = request.EmergencyPhone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                var patientResponse = new PatientApiResponse
                {
                    Id = patient.Id,
                    UserId = user.Id,
                    PatientNumber = patient.PatientNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    DateOfBirth = patient.DateOfBirth,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                return Ok(new UserCreationResponse 
                { 
                    Success = true, 
                    Message = "Patient created successfully", 
                    Data = patientResponse 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserCreationResponse 
                { 
                    Success = false, 
                    Error = "Internal server error: " + ex.Message 
                });
            }
        }

        [HttpPost("create-user")]
        public async Task<ActionResult<UserCreationResponse>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "User with this email already exists" 
                    });
                }

                // Validate patient-specific fields if provided
                DateTime? dateOfBirth = null;
                if (!string.IsNullOrEmpty(request.DateOfBirth))
                {
                    if (!DateTime.TryParse(request.DateOfBirth, out var parsedDate))
                    {
                        return BadRequest(new UserCreationResponse 
                        { 
                            Success = false, 
                            Error = "Invalid date of birth format. Use YYYY-MM-DD" 
                        });
                    }
                    dateOfBirth = parsedDate;
                }

                // Create new user
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new UserCreationResponse 
                    { 
                        Success = false, 
                        Error = "Failed to create user", 
                        Data = result.Errors 
                    });
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, request.Role);

                object? responseData = null;

                // Create appropriate record based on role
                if (request.Role.ToLower() == "patient" && !string.IsNullOrEmpty(request.DateOfBirth))
                {
                    // Create patient record
                    var patient = new Patient
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PatientNumber = $"PAT{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}",
                        DateOfBirth = dateOfBirth,
                        Address = request.Address,
                        EmergencyContactName = request.EmergencyContact,
                        EmergencyContactPhone = request.EmergencyPhone,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    responseData = new PatientResponse
                    {
                        Id = patient.Id,
                        UserId = user.Id,
                        PatientNumber = patient.PatientNumber,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        DateOfBirth = patient.DateOfBirth,
                        Address = patient.Address,
                        EmergencyContactName = patient.EmergencyContactName,
                        EmergencyContactPhone = patient.EmergencyContactPhone,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    };
                }
                else if (request.Role.ToLower() != "patient")
                {
                    // Create staff record
                    var staff = new Staff
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        StaffNumber = $"STF{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Staff.Add(staff);
                    await _context.SaveChangesAsync();

                    responseData = new { staffId = staff.Id, userId = user.Id };
                }

                return Ok(new UserCreationResponse 
                { 
                    Success = true, 
                    Message = $"User created successfully as {request.Role}", 
                    Data = responseData 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserCreationResponse 
                { 
                    Success = false, 
                    Error = "Internal server error: " + ex.Message 
                });
            }
        }

        [HttpPut("users/{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UpdateUserResponse>> UpdateUser(Guid userId, [FromBody] UpdateUserRequest? request)
        {
            try
            {
                _logger.LogInformation("=== UpdateUser START ===");
                _logger.LogInformation("UpdateUser called with userId: {UserId}", userId);
                _logger.LogInformation("Request is null: {IsNull}", request == null);
                
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Request body is null for userId: {UserId}", userId);
                    return BadRequest(new UpdateUserResponse 
                    { 
                        Message = "Request body is required"
                    });
                }

                // Log the incoming request for debugging
                try
                {
                    _logger.LogInformation("Request data: {RequestData}", System.Text.Json.JsonSerializer.Serialize(request));
                }
                catch (Exception serializationEx)
                {
                    _logger.LogWarning(serializationEx, "Failed to serialize request: {Message}", serializationEx.Message);
                }
                
                // Log model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    foreach (var error in ModelState)
                    {
                        foreach (var err in error.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                        {
                            _logger.LogWarning("ModelState Error - Key: {Key}, Error: {Error}", error.Key, err.ErrorMessage);
                        }
                    }
                }

                // Try to find user by User ID first
                var user = await _userManager.FindByIdAsync(userId.ToString());
                
                // If not found, try to find user through Staff ID
                if (user == null)
                {
                    _logger.LogWarning("User not found with User ID: {UserId}, trying Staff ID lookup...", userId);
                    var staff = await _context.Staff
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == userId);
                    
                    if (staff != null && staff.User != null)
                    {
                        user = staff.User;
                        _logger.LogInformation("Found user through Staff ID: {FirstName} {LastName}", staff.User.FirstName, staff.User.LastName);
                    }
                }
                
                // If still not found, try to find user through Patient ID
                if (user == null)
                {
                    _logger.LogWarning("User not found with Staff ID: {UserId}, trying Patient ID lookup...", userId);
                    var patient = await _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Id == userId);
                    
                    if (patient != null && patient.User != null)
                    {
                        user = patient.User;
                        _logger.LogInformation("Found user through Patient ID: {FirstName} {LastName}", patient.User.FirstName, patient.User.LastName);
                    }
                }

                if (user == null)
                {
                    _logger.LogWarning("User not found with any ID type: {UserId}", userId);
                    return NotFound(new UpdateUserResponse 
                    { 
                        Message = $"User not found with ID: {userId} (tried User ID, Staff ID, and Patient ID)",
                        User = new UserApiResponse
                        {
                            Id = Guid.Empty,
                            Email = "",
                            FirstName = "",
                            LastName = "",
                            Role = "",
                            IsActive = false,
                            CreatedAt = "",
                            UpdatedAt = "",
                            Phone = null,
                            DateOfBirth = null,
                            Address = null,
                            EmergencyContact = null,
                            EmergencyPhone = null
                        }
                    });
                }

                _logger.LogInformation("Found user: {FirstName} {LastName} ({Email})", user.FirstName, user.LastName, user.Email);

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new { 
                        type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        title = "One or more validation errors occurred.",
                        status = 400,
                        errors = errors
                    });
                }

                // Handle email update with uniqueness validation
                if (!string.IsNullOrEmpty(request.Email) && 
                    !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _userManager.FindByEmailAsync(request.Email);
                    if (emailExists != null && emailExists.Id != userId)
                    {
                        return BadRequest(new UpdateUserResponse 
                        { 
                            Message = "Email address is already in use by another account"
                        });
                    }
                }

                // Update basic user information (only update if provided)
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.Email))
                {
                    user.Email = request.Email;
                    user.UserName = request.Email; // Update username to match email
                }
                
                // Handle optional fields - accept empty strings and null
                if (request.Phone != null)
                    user.PhoneNumber = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone;
                
                if (!string.IsNullOrEmpty(request.DateOfBirth))
                {
                    if (DateTime.TryParse(request.DateOfBirth, out var dateOfBirth))
                    {
                        user.DateOfBirth = dateOfBirth;
                    }
                }
                else if (request.DateOfBirth == "")
                {
                    user.DateOfBirth = null; // Allow clearing date of birth
                }
                
                // Handle address - accept empty strings
                if (request.Address != null)
                {
                    user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address;
                }
                
                // Handle emergency contact - accept empty strings
                if (request.EmergencyContact != null)
                {
                    user.EmergencyContact = string.IsNullOrWhiteSpace(request.EmergencyContact) ? null : request.EmergencyContact;
                }
                
                // Handle emergency phone - accept empty strings
                if (request.EmergencyPhone != null)
                {
                    user.EmergencyPhone = string.IsNullOrWhiteSpace(request.EmergencyPhone) ? null : request.EmergencyPhone;
                }
                
                user.UpdatedAt = DateTime.UtcNow;

                // Update active status if provided
                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }

                // Ensure SecurityStamp is set (required for Identity)
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }

                // Update user in database
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user. Errors: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    return BadRequest(new UpdateUserResponse 
                    { 
                        Message = "Failed to update user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description))
                    });
                }

                _logger.LogInformation("Successfully updated user. IsActive: {IsActive}", user.IsActive);

                // Update associated Patient/Staff record if exists
                var associatedPatient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (associatedPatient != null)
                {
                    // Handle address for Patient record (flat string format)
                    if (request.Address != null)
                    {
                        associatedPatient.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address;
                    }
                    
                    // Handle emergency contact for Patient record
                    if (request.EmergencyContact != null)
                    {
                        // Parse emergency contact string if it contains structured data
                        // For now, store as-is in EmergencyContactName
                        associatedPatient.EmergencyContactName = string.IsNullOrWhiteSpace(request.EmergencyContact) ? null : request.EmergencyContact;
                    }
                    
                    // Handle emergency phone for Patient record
                    if (request.EmergencyPhone != null)
                    {
                        associatedPatient.EmergencyContactPhone = string.IsNullOrWhiteSpace(request.EmergencyPhone) ? null : request.EmergencyPhone;
                    }
                    
                    associatedPatient.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated associated patient record");
                }

                // Update associated Staff record if exists
                var associatedStaff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (associatedStaff != null && request.IsActive.HasValue)
                {
                    associatedStaff.IsActive = request.IsActive.Value;
                    associatedStaff.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated associated staff record");
                }

                // Update role if provided
                if (!string.IsNullOrEmpty(request.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(user, request.Role);
                    _logger.LogInformation("Successfully updated user role to: {Role}", request.Role);
                }

                // Get updated user roles
                var userRoles = await _userManager.GetRolesAsync(user);
                var displayRole = userRoles.FirstOrDefault() ?? "user";

                return Ok(new UpdateUserResponse
                {
                    Message = "User updated successfully",
                    User = new UserApiResponse
                    {
                        Id = user.Id,
                        Email = user.Email ?? "",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = displayRole,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Phone = user.PhoneNumber,
                        DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                        Address = user.Address,
                        EmergencyContact = user.EmergencyContact,
                        EmergencyPhone = user.EmergencyPhone
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateUser: {Message}", ex.Message);
                return StatusCode(500, new UpdateUserResponse 
                { 
                    Message = "Internal server error: " + ex.Message,
                    User = new UserApiResponse
                    {
                        Id = Guid.Empty,
                        Email = "",
                        FirstName = "",
                        LastName = "",
                        Role = "",
                        IsActive = false,
                        CreatedAt = "",
                        UpdatedAt = ""
                    }
                });
            }
        }

        [HttpPut("users/{userId}/status")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateUserStatus(Guid userId, [FromBody] UpdateStaffStatusRequest request)
        {
            try
            {
                _logger.LogInformation("UpdateUserStatus called with userId: {UserId}", userId);
                _logger.LogInformation("Request data: {RequestData}", System.Text.Json.JsonSerializer.Serialize(request));

                // Validate request
                if (request == null)
                {
                    return BadRequest(new { error = "Request body is required" });
                }

                // Try to find user by User ID first
                var user = await _userManager.FindByIdAsync(userId.ToString());
                
                // If not found, try to find user through Staff ID
                if (user == null)
                {
                    _logger.LogWarning("User not found with User ID: {UserId}, trying Staff ID lookup...", userId);
                    var staff = await _context.Staff
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == userId);
                    
                    if (staff != null && staff.User != null)
                    {
                        user = staff.User;
                        _logger.LogInformation("Found user through Staff ID: {FirstName} {LastName}", staff.User.FirstName, staff.User.LastName);
                    }
                }
                
                // If still not found, try to find user through Patient ID
                if (user == null)
                {
                    _logger.LogWarning("User not found with Staff ID: {UserId}, trying Patient ID lookup...", userId);
                    var patient = await _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Id == userId);
                    
                    if (patient != null && patient.User != null)
                    {
                        user = patient.User;
                        _logger.LogInformation("Found user through Patient ID: {FirstName} {LastName}", patient.User.FirstName, patient.User.LastName);
                    }
                }

                if (user == null)
                {
                    _logger.LogWarning("User not found with any ID type: {UserId}", userId);
                    return NotFound(new { 
                        error = $"User not found with ID: {userId} (tried User ID, Staff ID, and Patient ID)" 
                    });
                }

                // Ensure user is tracked by DbContext (attach if not tracked)
                if (_context.Entry(user).State == EntityState.Detached)
                {
                    _context.Users.Attach(user);
                }

                _logger.LogInformation("Found user: {FirstName} {LastName} ({Email})", user.FirstName, user.LastName, user.Email);
                _logger.LogInformation("Current IsActive: {CurrentIsActive}, Requested IsActive: {RequestedIsActive}", user.IsActive, request.IsActive);

                // Update IsActive status
                user.IsActive = request.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                // Ensure SecurityStamp is set (required for Identity)
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }

                // Mark entity as modified in DbContext to ensure changes are tracked
                _context.Entry(user).Property(u => u.IsActive).IsModified = true;
                _context.Entry(user).Property(u => u.UpdatedAt).IsModified = true;

                // Save changes using UserManager first (for Identity updates)
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user status via UserManager. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest(new { 
                        error = "Failed to update user status",
                        details = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Explicitly save changes to DbContext to ensure persistence
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation("DbContext.SaveChangesAsync result: {ChangesSaved} changes saved", saveResult);

                // Verify the change was persisted by reloading from database
                await _context.Entry(user).ReloadAsync();
                
                _logger.LogInformation("Successfully updated user status. IsActive after reload: {IsActive}", user.IsActive);

                // Get user roles for response
                var userRoles = await _userManager.GetRolesAsync(user);
                var displayRole = userRoles.FirstOrDefault() ?? "user";

                return Ok(new 
                { 
                    id = user.Id,
                    email = user.Email ?? "",
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    isActive = user.IsActive,
                    role = displayRole,
                    message = user.IsActive ? "User activated successfully" : "User deactivated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateUserStatus: {Message}", ex.Message);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("reports/appointment-stats")]
        public async Task<ActionResult<AppointmentStatsDto>> GetAppointmentStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                var appointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var stats = new AppointmentStatsDto
                {
                    StartDate = start,
                    EndDate = end,
                    TotalAppointments = appointments.Count,
                    CompletedAppointments = appointments.Count(a => a.Status == "completed"),
                    CancelledAppointments = appointments.Count(a => a.Status == "cancelled"),
                    PendingAppointments = appointments.Count(a => a.Status == "pending" || a.Status == "confirmed")
                };

                return Ok(stats);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get ALL users in the system (staff + patients) for admin user management with pagination and search
        /// This endpoint supports server-side pagination, search, filtering, and sorting for optimal performance
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult> GetUsers([FromQuery] UserSearchRequest request)
        {
            try
            {
                // Check if frontend wants simple array format
                if (request.Search == "simple" || request.Search == "array" || request.Search == "all")
                {
                    var allUsersSimple = await _context.Users
                        .Include(u => u.Patient)
                        .Include(u => u.Staff)
                        .ToListAsync();

                    var usersWithRolesSimple = new List<object>();

                    foreach (var user in allUsersSimple)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);

                        string displayRole;
                        if (userRoles.Contains("admin"))
                            displayRole = "admin";
                        else if (userRoles.Contains("doctor"))
                            displayRole = "doctor";
                        else if (userRoles.Contains("staff"))
                            displayRole = "staff";
                        else if (userRoles.Contains("patient"))
                            displayRole = "patient";
                        else if (user.Staff != null)
                            displayRole = "staff";
                        else if (user.Patient != null)
                            displayRole = "patient";
                        else
                            displayRole = "user";

                        usersWithRolesSimple.Add(new
                        {
                            id = user.Id,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            email = user.Email,
                            phoneNumber = user.PhoneNumber,
                            role = displayRole,
                            isActive = user.IsActive,
                            createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            updatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            phone = user.PhoneNumber,
                            dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                            address = user.Address,
                            emergencyContact = user.EmergencyContact,
                            emergencyPhone = user.EmergencyPhone
                        });
                    }

                    return Ok(usersWithRolesSimple);
                }

                // If no query parameters provided, return simple array for frontend compatibility
                if (string.IsNullOrEmpty(request.Search) && 
                    string.IsNullOrEmpty(request.Role) && 
                    string.IsNullOrEmpty(request.Status))
                {
                    var allUsers = await _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .ToListAsync();

                    var usersWithRolesArray = new List<object>();

                    foreach (var user in allUsers)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);

                        string displayRole;
                        if (userRoles.Contains("admin"))
                            displayRole = "admin";
                        else if (userRoles.Contains("doctor"))
                            displayRole = "doctor";
                        else if (userRoles.Contains("staff"))
                            displayRole = "staff";
                        else if (userRoles.Contains("patient"))
                            displayRole = "patient";
                        else if (user.Staff != null)
                            displayRole = "staff";
                        else if (user.Patient != null)
                            displayRole = "patient";
                        else
                            displayRole = "user";

                        usersWithRolesArray.Add(new
                        {
                            id = user.Id,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            email = user.Email,
                            phoneNumber = user.PhoneNumber,
                            role = displayRole,
                            isActive = user.IsActive,
                            createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            updatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            phone = user.PhoneNumber,
                            dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                            address = user.Address,
                            emergencyContact = user.EmergencyContact,
                            emergencyPhone = user.EmergencyPhone
                        });
                    }

                    return Ok(usersWithRolesArray);
                }

                // Otherwise, return paginated response
                // Validate request parameters
                if (!ModelState.IsValid)
                {
                    return BadRequest(new UserListResponse 
                    { 
                        Success = false, 
                        Error = "Invalid request parameters",
                        Filters = new FilterInfo
                        {
                            Search = request.Search,
                            Role = request.Role,
                            Status = request.Status
                        }
                    });
                }

                // Build the base query
                var query = _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(request.Search))
                {
                    var searchTerm = request.Search.ToLower();
                    query = query.Where(u => 
                        (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                        (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchTerm)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))
                    );
                }

                // Apply role filter
                if (!string.IsNullOrEmpty(request.Role) && request.Role != "all")
                {
                    // For role filtering, we need to check the actual roles in the database
                    // This is more complex due to ASP.NET Identity role system
                    // We'll filter after getting the roles
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(request.Status) && request.Status != "all")
                {
                    var isActive = request.Status.ToLower() == "active";
                    query = query.Where(u => u.IsActive == isActive);
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "firstname" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                    "lastname" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                    "email" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                    "createdat" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                    "updatedat" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.UpdatedAt) : query.OrderBy(u => u.UpdatedAt),
                    _ => request.SortOrder == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
                };

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var users = await query
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .ToListAsync();

                // Process users and get roles
                var usersWithRoles = new List<AdminUserResponse>();

                foreach (var user in users)
                {
                    // Get the user's actual roles from the database
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Determine role based on actual roles and records
                    string displayRole;
                    if (userRoles.Contains("admin"))
                        displayRole = "admin";
                    else if (userRoles.Contains("doctor"))
                        displayRole = "doctor";
                    else if (userRoles.Contains("staff"))
                        displayRole = "staff";
                    else if (userRoles.Contains("patient"))
                        displayRole = "patient";
                    else if (user.Staff != null)
                        displayRole = "staff";
                    else if (user.Patient != null)
                        displayRole = "patient";
                    else
                        displayRole = "user";

                    // Apply role filter after getting roles (if specified)
                    if (!string.IsNullOrEmpty(request.Role) && request.Role != "all" && displayRole != request.Role.ToLower())
                    {
                        continue; // Skip this user if role doesn't match
                    }

                    var userResponse = new AdminUserResponse
                    {
                        Id = user.Id,
                        Email = user.Email ?? "",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Phone = user.PhoneNumber,
                        DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                        Address = user.Address,
                        EmergencyContact = user.EmergencyContact,
                        EmergencyPhone = user.EmergencyPhone,
                        Role = displayRole,
                        ActualRoles = userRoles.ToList(),
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        PatientInfo = user.Patient != null ? new PatientInfo
                        {
                            PatientId = user.Patient.Id,
                            PatientNumber = user.Patient.PatientNumber,
                            DateOfBirth = user.Patient.DateOfBirth,
                            Address = user.Patient.Address,
                            EmergencyContact = user.Patient.EmergencyContactName,
                            EmergencyPhone = user.Patient.EmergencyContactPhone
                        } : null,
                        StaffInfo = user.Staff != null ? new StaffInfo
                        {
                            StaffId = user.Staff.Id,
                            StaffNumber = user.Staff.StaffNumber
                        } : null
                    };

                    usersWithRoles.Add(userResponse);
                }

                // Calculate pagination info
                var totalPages = (int)Math.Ceiling((double)totalCount / request.Limit);

                var response = new UserListResponse
                {
                    Users = usersWithRoles,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = request.Page,
                        TotalPages = totalPages,
                        TotalItems = totalCount,
                        ItemsPerPage = request.Limit,
                        HasNextPage = request.Page < totalPages,
                        HasPreviousPage = request.Page > 1
                    },
                    Filters = new FilterInfo
                    {
                        Search = request.Search,
                        Role = request.Role,
                        Status = request.Status
                    },
                    Success = true
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserListResponse 
                { 
                    Success = false, 
                    Error = "Internal server error: " + ex.Message,
                    Filters = new FilterInfo
                    {
                        Search = request.Search,
                        Role = request.Role,
                        Status = request.Status
                    }
                });
            }
        }

        /// <summary>
        /// Simple test endpoint to verify route discovery
        /// </summary>
        [HttpGet("array-test")]
        [AllowAnonymous]
        public ActionResult TestArrayRoute()
        {
            return Ok(new[] { 
                new { id = "1", name = "Test User 1", role = "admin" },
                new { id = "2", name = "Test User 2", role = "patient" }
            });
        }

        /// <summary>
        /// Get all users as a simple array (for frontend compatibility)
        /// This endpoint returns a direct array without pagination metadata
        /// </summary>
        [HttpGet("all-users")]
        public async Task<ActionResult<List<object>>> GetAllUsersSimple()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .ToListAsync();

                var usersWithRoles = new List<object>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    string displayRole;
                    if (userRoles.Contains("admin"))
                        displayRole = "admin";
                    else if (userRoles.Contains("doctor"))
                        displayRole = "doctor";
                    else if (userRoles.Contains("staff"))
                        displayRole = "staff";
                    else if (userRoles.Contains("patient"))
                        displayRole = "patient";
                    else if (user.Staff != null)
                        displayRole = "staff";
                    else if (user.Patient != null)
                        displayRole = "patient";
                    else
                        displayRole = "user";

                    usersWithRoles.Add(new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        role = displayRole,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        updatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        phone = user.PhoneNumber,
                        dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                        address = user.Address,
                        emergencyContact = user.EmergencyContact,
                        emergencyPhone = user.EmergencyPhone
                    });
                }

                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users simple: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Get users as simple array (alternative to paginated version)
        /// This endpoint returns a direct array for frontend compatibility
        /// </summary>
        [HttpGet("simple-users")]
        public async Task<ActionResult<List<object>>> GetUsersSimple()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .ToListAsync();

                var usersWithRoles = new List<object>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    string displayRole;
                    if (userRoles.Contains("admin"))
                        displayRole = "admin";
                    else if (userRoles.Contains("doctor"))
                        displayRole = "doctor";
                    else if (userRoles.Contains("staff"))
                        displayRole = "staff";
                    else if (userRoles.Contains("patient"))
                        displayRole = "patient";
                    else if (user.Staff != null)
                        displayRole = "staff";
                    else if (user.Patient != null)
                        displayRole = "patient";
                    else
                        displayRole = "user";

                    usersWithRoles.Add(new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        role = displayRole,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        updatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        phone = user.PhoneNumber,
                        dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                        address = user.Address,
                        emergencyContact = user.EmergencyContact,
                        emergencyPhone = user.EmergencyPhone
                    });
                }

                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users simple: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("services")]
        public async Task<ActionResult<object>> GetServices()
        {
            try
            {
                var services = await _context.Services
                    .Where(s => s.IsActive)
                    .Select(s => new
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
                        Price = s.Price,
                        IsActive = s.IsActive
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "Internal server error: " + ex.Message });
            }
        }

        [HttpPost("walkin-appointment")]
        public async Task<ActionResult<object>> CreateWalkInAppointment([FromBody] AdminWalkInAppointmentRequest request)
        {
            try
            {
                // Validate patient exists
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId);
                if (patient == null)
                {
                    return BadRequest(new { success = false, error = "Patient not found" });
                }

                // Validate service exists
                var service = await _context.Services.FindAsync(request.ServiceId);
                if (service == null)
                {
                    return BadRequest(new { success = false, error = "Service not found" });
                }

                // Validate staff exists
                var staff = await _context.Staff.FindAsync(request.StaffId);
                if (staff == null)
                {
                    return BadRequest(new { success = false, error = "Staff member not found" });
                }

                // Parse appointment date and times
                if (!DateTime.TryParse(request.AppointmentDate, out var appointmentDate))
                {
                    return BadRequest(new { success = false, error = "Invalid appointment date format. Use YYYY-MM-DD" });
                }

                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                {
                    return BadRequest(new { success = false, error = "Invalid start time format. Use HH:mm:ss or HH:mm" });
                }

                TimeOnly endTime;
                if (string.IsNullOrEmpty(request.EndTime))
                {
                    // Calculate end time based on service duration
                    endTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                }
                else
                {
                    if (!TimeOnly.TryParse(request.EndTime, out endTime))
                    {
                        return BadRequest(new { success = false, error = "Invalid end time format. Use HH:mm:ss or HH:mm" });
                    }
                }

                // Check for time conflicts
                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.AppointmentDate == appointmentDate &&
                               a.StaffId == request.StaffId &&
                               a.Status != "cancelled" &&
                               ((a.StartTime <= startTime && a.EndTime > startTime) ||
                                (a.StartTime < endTime && a.EndTime >= endTime) ||
                                (a.StartTime >= startTime && a.EndTime <= endTime)))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    return BadRequest(new { success = false, error = "Staff member is not available at this time" });
                }

                // Create the walk-in appointment
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = request.PatientId,
                    ServiceId = request.ServiceId,
                    StaffId = request.StaffId,
                    AppointmentDate = appointmentDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = "walkin",
                    BookingType = "walkin",
                    IsWalkIn = true,
                    ServicePrice = service.Price,
                    PaymentStatus = "pending",
                    Notes = request.Notes,
                    ApprovalStatus = ApprovalStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Walk-in appointment created successfully",
                    data = new
                    {
                        appointmentId = appointment.Id,
                        patientId = appointment.PatientId,
                        patientName = $"{patient.User.FirstName} {patient.User.LastName}",
                        serviceName = service.Name,
                        staffName = staff.User?.FirstName + " " + staff.User?.LastName,
                        appointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                        startTime = appointment.StartTime.ToString("HH:mm"),
                        endTime = appointment.EndTime.ToString("HH:mm"),
                        status = appointment.Status
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "Internal server error: " + ex.Message });
            }
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles
                    .Select(r => r.Name ?? "")
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("users/{userId}/role")]
        public async Task<ActionResult> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                
                // Remove all current roles
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                
                // Add new role
                var result = await _userManager.AddToRoleAsync(user, request.NewRole);
                if (!result.Succeeded)
                {
                    return BadRequest(new { error = "Failed to update user role" });
                }

                return Ok(new { message = "User role updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        [HttpGet("reports/revenue")]
        public async Task<ActionResult<RevenueReportDto>> GetRevenueReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] Guid? serviceId)
        {
            try
            {
                var start = startDate?.Date ?? DateTime.Today.AddDays(-30);
                var end = endDate?.Date ?? DateTime.Today;

                if (start > end)
                {
                    return BadRequest(new ApiErrorDto
                    {
                        Error = "Invalid date range",
                        Message = "Start date must be before end date",
                        Code = "INVALID_DATE_RANGE"
                    });
                }

                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end && a.Status == "completed")
                    .Where(a => !serviceId.HasValue || a.ServiceId == serviceId.Value)
                    .ToListAsync();

                var serviceBreakdown = appointments
                    .GroupBy(a => a.Service)
                    .Select(g => new ServiceRevenueDto
                    {
                        ServiceId = g.Key.Id,
                        ServiceName = g.Key.Name,
                        AppointmentCount = g.Count(),
                        Revenue = g.Sum(x => x.ServicePrice ?? 0)
                    })
                    .ToList();

                // Revenue by month (yyyy-MM)
                var revenueByMonth = appointments
                    .GroupBy(a => a.AppointmentDate.ToString("yyyy-MM"))
                    .Select(g => new RevenueByMonthDto
                    {
                        Month = g.Key,
                        Revenue = g.Sum(a => a.ServicePrice ?? 0),
                        AppointmentCount = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                // Revenue by week (ISO week starting Monday). We derive week start date.
                static DateTime WeekStart(DateTime d)
                {
                    var diff = (7 + (d.DayOfWeek - DayOfWeek.Monday)) % 7;
                    return d.AddDays(-diff).Date;
                }

                var revenueByWeek = appointments
                    .GroupBy(a => WeekStart(a.AppointmentDate))
                    .Select(g => new RevenueByWeekDto
                    {
                        WeekStart = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Sum(a => a.ServicePrice ?? 0),
                        AppointmentCount = g.Count()
                    })
                    .OrderBy(x => x.WeekStart)
                    .ToList();

                var report = new RevenueReportDto
                {
                    StartDate = start,
                    EndDate = end,
                    TotalAppointments = appointments.Count,
                    TotalRevenue = serviceBreakdown.Sum(s => s.Revenue),
                    ServiceBreakdown = serviceBreakdown,
                    RevenueByMonth = revenueByMonth,
                    RevenueByWeek = revenueByWeek
                };

                return Ok(report);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("reports/service-usage")]
        public async Task<ActionResult<ServiceUsageReportDto>> GetServiceUsageReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate?.Date ?? DateTime.Today.AddDays(-30);
                var end = endDate?.Date ?? DateTime.Today;

                if (start > end)
                {
                    return BadRequest(new ApiErrorDto
                    {
                        Error = "Invalid date range",
                        Message = "Start date must be before end date",
                        Code = "INVALID_DATE_RANGE"
                    });
                }

                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var totalAppointments = appointments.Count;

                var items = appointments
                    .GroupBy(a => a.ServiceId)
                    .Select(g => new ServiceUsageItemDto
                    {
                        ServiceId = g.Key,
                        ServiceName = g.First().Service.Name,
                        UsageCount = g.Count(),
                        PercentageOfTotal = totalAppointments > 0 ? Math.Round((double)g.Count() / totalAppointments * 100, 2) : 0,
                        AveragePrice = g.Count() > 0 ? Math.Round(g.Average(x => x.ServicePrice ?? 0), 2) : 0
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .ToList();

                var response = new ServiceUsageReportDto
                {
                    Services = items,
                    TotalAppointments = totalAppointments,
                    DateRange = new DateRangeDto { StartDate = start.ToString("yyyy-MM-dd"), EndDate = end.ToString("yyyy-MM-dd") }
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("reports/comprehensive")]
        public async Task<ActionResult<ComprehensiveAnalyticsDto>> GetComprehensiveAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool includeCharts = false)
        {
            try
            {
                var start = startDate?.Date ?? DateTime.Today.AddDays(-30);
                var end = endDate?.Date ?? DateTime.Today;

                if (start > end)
                {
                    return BadRequest(new ApiErrorDto
                    {
                        Error = "Invalid date range",
                        Message = "Start date must be before end date",
                        Code = "INVALID_DATE_RANGE"
                    });
                }

                // Appointment stats (reuse logic similar to GetAppointmentStats)
                var appts = await _context.Appointments
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var stats = new AppointmentStatsDto
                {
                    StartDate = start,
                    EndDate = end,
                    TotalAppointments = appts.Count,
                    CompletedAppointments = appts.Count(a => a.Status == "completed"),
                    CancelledAppointments = appts.Count(a => a.Status == "cancelled"),
                    PendingAppointments = appts.Count(a => a.Status == "pending" || a.Status == "confirmed")
                };

                // Service usage summary (reuse service usage computation)
                var apptsWithServices = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var totalAppt = apptsWithServices.Count;
                var serviceUsage = apptsWithServices
                    .GroupBy(a => a.ServiceId)
                    .Select(g => new ServiceUsageItemDto
                    {
                        ServiceId = g.Key,
                        ServiceName = g.First().Service.Name,
                        UsageCount = g.Count(),
                        PercentageOfTotal = totalAppt > 0 ? Math.Round((double)g.Count() / totalAppt * 100, 2) : 0,
                        AveragePrice = g.Count() > 0 ? Math.Round(g.Average(x => x.ServicePrice ?? 0), 2) : 0
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .ToList();

                // Revenue data (reuse revenue computation)
                var revenueResponse = await GetRevenueReport(startDate, endDate, null);
                if (revenueResponse.Result is ObjectResult objectResult && objectResult.StatusCode >= 400)
                {
                    return objectResult;
                }
                var revenueData = (revenueResponse.Value as RevenueReportDto)!;

                var result = new ComprehensiveAnalyticsDto
                {
                    AppointmentStats = stats,
                    ServiceUsage = serviceUsage,
                    RevenueData = revenueData,
                    DateRange = new DateRangeDto { StartDate = start.ToString("yyyy-MM-dd"), EndDate = end.ToString("yyyy-MM-dd") },
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult<object> TestEndpoint()
        {
            return Ok(new { message = "AdminController is working", timestamp = DateTime.UtcNow });
        }

        [HttpGet("test-auth")]
        [Authorize(Roles = "admin")]
        public ActionResult<object> TestAuthEndpoint()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            
            return Ok(new { 
                message = "AdminController auth test", 
                userId = userId,
                roles = userRoles,
                isAuthenticated = isAuthenticated,
                timestamp = DateTime.UtcNow 
            });
        }

        [HttpGet("test-service")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestServiceEndpoint()
        {
            try
            {
                var doctors = await _adminDoctorService.GetAllDoctorsAsync();
                return Ok(new { message = "AdminDoctorService is working", doctorCount = doctors.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Service error", message = ex.Message });
            }
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<AdminDoctorListResponse>> GetAllDoctors()
        {
            try
            {
                var doctors = await _adminDoctorService.GetAllDoctorsAsync();
                
                return Ok(new AdminDoctorListResponse
                {
                    Success = true,
                    Doctors = doctors,
                    TotalCount = doctors.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AdminDoctorListResponse
                {
                    Success = false,
                    Error = "Internal server error: " + ex.Message
                });
            }
        }

        [HttpGet("doctors/{doctorId}/schedule")]
        public async Task<ActionResult<AdminDoctorScheduleResponse>> GetDoctorSchedule(Guid doctorId)
        {
            try
            {
                // Get current user ID for authorization check
                var currentUserId = User.FindFirst("sub")?.Value;
                if (currentUserId == null || !Guid.TryParse(currentUserId, out var userId))
                {
                    return Unauthorized(new AdminDoctorScheduleResponse
                    {
                        Success = false,
                        Error = "Unauthorized access"
                    });
                }

                // Check if user can manage this doctor's schedule
                var canManage = await _adminDoctorService.CanManageDoctorScheduleAsync(userId, doctorId);
                if (!canManage)
                {
                    return Forbid();
                }

                // Get doctor information
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new AdminDoctorScheduleResponse
                    {
                        Success = false,
                        Error = "Doctor not found"
                    });
                }

                // Get doctor's shift schedule
                var schedule = await _adminDoctorService.GetDoctorShiftScheduleAsync(doctorId);

                return Ok(new AdminDoctorScheduleResponse
                {
                    Success = true,
                    DoctorId = doctorId,
                    DoctorName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                    Schedule = schedule
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AdminDoctorScheduleResponse
                {
                    Success = false,
                    Error = "Internal server error: " + ex.Message
                });
            }
        }

        [HttpGet("test-simple")]
        [AllowAnonymous]
        public ActionResult<object> TestSimple()
        {
            return Ok(new { message = "AdminController is working", timestamp = DateTime.UtcNow });
        }

        [HttpGet("debug-auth")]
        [Authorize]
        public ActionResult<object> DebugAuth()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                
                return Ok(new
                {
                    success = true,
                    userId = userId,
                    roles = roles,
                    isAuthenticated = isAuthenticated,
                    claims = claims,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "DEBUG_ERROR", message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("doctors/{doctorId}/shifts")]
        [AllowAnonymous] // Temporarily remove authorization to test
        public async Task<ActionResult<object>> GetDoctorShifts(Guid doctorId)
        {
            try
            {
                // Get doctor information
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { error = "DOCTOR_NOT_FOUND", message = "Doctor not found" });
                }

                // Get shift schedules directly from database to match BookingController format
                var shifts = await _context.ShiftSchedules
                    .Where(s => s.DoctorId == doctorId)
                    .OrderBy(s => s.DayOfWeek)
                    .Select(s => new
                    {
                        id = s.Id,
                        dayOfWeek = s.DayOfWeek,
                        startTime = s.StartTime.ToString(@"hh\:mm"),
                        endTime = s.EndTime.ToString(@"hh\:mm"),
                        isActive = s.IsActive,
                        breakStartTime = (string?)null,
                        breakEndTime = (string?)null,
                        effectiveFrom = (string?)null,
                        effectiveTo = (string?)null
                    }).ToListAsync();

                // Get next available date
                var nextAvailableDate = DateTime.Today.AddDays(1); // Simple fallback

                var response = new
                {
                    doctorId = doctorId,
                    doctorName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                    shifts = shifts,
                    nextAvailableDate = nextAvailableDate,
                    lastUpdated = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "INTERNAL_ERROR", message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("doctors/{doctorId}/shifts")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<object>> UpdateDoctorShifts(Guid doctorId, [FromBody] UpdateShiftScheduleRequest request)
        {
            try
            {
                // Validate request
                if (request == null || request.Shifts == null || !request.Shifts.Any())
                {
                    return BadRequest(new { error = "INVALID_REQUEST", message = "Shifts data is required" });
                }

                // Get doctor info
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { error = "DOCTOR_NOT_FOUND", message = "Doctor not found" });
                }

                // Get existing shifts
                var existingShifts = await _context.ShiftSchedules
                    .Where(s => s.DoctorId == doctorId)
                    .ToListAsync();

                var updatedShifts = new List<object>();

                // Update or create shifts
                foreach (var shiftInfo in request.Shifts)
                {
                    // Validate shift data
                    if (string.IsNullOrWhiteSpace(shiftInfo.DayOfWeek))
                    {
                        return BadRequest(new { error = "INVALID_SHIFT_DATA", message = "DayOfWeek is required for all shifts" });
                    }

                    if (string.IsNullOrWhiteSpace(shiftInfo.StartTime) || string.IsNullOrWhiteSpace(shiftInfo.EndTime))
                    {
                        return BadRequest(new { error = "INVALID_SHIFT_DATA", message = "StartTime and EndTime are required for all shifts" });
                    }

                    // Parse times - handle both "HH:mm" and "HH:mm:ss" formats
                    if (!TimeSpan.TryParse(shiftInfo.StartTime, out var startTime))
                    {
                        return BadRequest(new { error = "INVALID_TIME_FORMAT", message = $"Invalid start time format: {shiftInfo.StartTime}. Use HH:mm or HH:mm:ss format" });
                    }

                    if (!TimeSpan.TryParse(shiftInfo.EndTime, out var endTime))
                    {
                        return BadRequest(new { error = "INVALID_TIME_FORMAT", message = $"Invalid end time format: {shiftInfo.EndTime}. Use HH:mm or HH:mm:ss format" });
                    }

                    if (endTime <= startTime)
                    {
                        return BadRequest(new { error = "INVALID_TIME_RANGE", message = $"End time must be after start time for {shiftInfo.DayOfWeek}" });
                    }

                    var existingShift = existingShifts.FirstOrDefault(s => s.DayOfWeek == shiftInfo.DayOfWeek);
                    
                    if (existingShift != null)
                    {
                        // Update existing shift
                        existingShift.StartTime = startTime;
                        existingShift.EndTime = endTime;
                        existingShift.IsActive = shiftInfo.IsActive;
                        existingShift.UpdatedAt = DateTime.UtcNow;

                        updatedShifts.Add(new
                        {
                            id = existingShift.Id,
                            dayOfWeek = existingShift.DayOfWeek,
                            startTime = existingShift.StartTime.ToString(@"hh\:mm"),
                            endTime = existingShift.EndTime.ToString(@"hh\:mm"),
                            isActive = existingShift.IsActive
                        });
                    }
                    else
                    {
                        // Create new shift
                        var newShift = new ShiftSchedule
                        {
                            Id = Guid.NewGuid(),
                            DoctorId = doctorId,
                            DayOfWeek = shiftInfo.DayOfWeek,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsActive = shiftInfo.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.ShiftSchedules.Add(newShift);

                        updatedShifts.Add(new
                        {
                            id = newShift.Id,
                            dayOfWeek = newShift.DayOfWeek,
                            startTime = newShift.StartTime.ToString(@"hh\:mm"),
                            endTime = newShift.EndTime.ToString(@"hh\:mm"),
                            isActive = newShift.IsActive
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    doctorId = doctorId,
                    message = "Shift schedule updated successfully",
                    shifts = updatedShifts
                });
            }
            catch (FormatException ex)
            {
                return BadRequest(new { error = "INVALID_TIME_FORMAT", message = "Invalid time format. Use HH:mm or HH:mm:ss format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "INTERNAL_ERROR", message = ex.Message });
            }
        }
    }
}

