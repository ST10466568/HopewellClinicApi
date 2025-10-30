using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Attributes;
using HopewellClinicApi.Models;
using System.Text.Json;

namespace HopewellClinicApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
// [JwtAuthorize] // Temporarily disabled for testing
    public class PatientsController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientsController(HopewellDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous] // Allow patient self-registration
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
                        Error = string.Join(", ", result.Errors.Select(e => e.Description)) 
                    });
                }

                // Add patient role
                await _userManager.AddToRoleAsync(user, "patient");

                // Generate patient number
                var patientNumber = $"PAT{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}";

                // Create patient record
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PatientNumber = patientNumber,
                    DateOfBirth = dateOfBirth,
                    Address = request.Address,
                    EmergencyContactName = request.EmergencyContact,
                    EmergencyContactPhone = request.EmergencyPhone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                var patientResponse = new PatientResponse
                {
                    Id = patient.Id,
                    UserId = patient.UserId,
                    PatientNumber = patient.PatientNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.PhoneNumber ?? "",
                    Email = user.Email ?? "",
                    DateOfBirth = patient.DateOfBirth,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    CreatedAt = patient.CreatedAt,
                    UpdatedAt = patient.UpdatedAt
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
                Console.WriteLine($"Error creating patient: {ex.Message}");
                return StatusCode(500, new UserCreationResponse 
                { 
                    Success = false, 
                    Error = "Internal server error: " + ex.Message 
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientResponse>>> GetPatients()
        {
            try
            {
                var patients = await _context.Patients
                    .Include(p => p.User)
                    .Select(p => new PatientResponse
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        PatientNumber = p.PatientNumber,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Phone = p.User.PhoneNumber ?? "",
                        Email = p.User.Email ?? "",
                        DateOfBirth = p.DateOfBirth,
                        Address = p.Address,
                        EmergencyContactName = p.EmergencyContactName,
                        EmergencyContactPhone = p.EmergencyContactPhone,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPatients: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetPatient(Guid id)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                {
                    return NotFound(new { error = "Patient not found" });
                }

                var response = new PatientResponse
                {
                    Id = patient.Id,
                    UserId = patient.UserId,
                    PatientNumber = patient.PatientNumber,
                    FirstName = patient.User.FirstName,
                    LastName = patient.User.LastName,
                    Phone = patient.User.PhoneNumber ?? "",
                    Email = patient.User.Email ?? "",
                    DateOfBirth = patient.DateOfBirth,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    CreatedAt = patient.CreatedAt,
                    UpdatedAt = patient.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                {
                    return NotFound(new { error = "Patient not found" });
                }

                // Update User record fields
                if (!string.IsNullOrEmpty(request.FirstName))
                    patient.User.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    patient.User.LastName = request.LastName;

                // Handle email update with uniqueness validation
                if (!string.IsNullOrEmpty(request.Email) && 
                    !string.Equals(patient.User.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _userManager.FindByEmailAsync(request.Email);
                    if (emailExists != null && emailExists.Id != patient.UserId)
                    {
                        return BadRequest(new { 
                            error = "Email Exists", 
                            message = "Email address is already in use by another account" 
                        });
                    }
                    
                    patient.User.Email = request.Email;
                    patient.User.UserName = request.Email; // Update username to match email
                }

                // Handle phone update
                var phoneToUpdate = request.Phone ?? request.PhoneNumber;
                if (!string.IsNullOrEmpty(phoneToUpdate))
                    patient.User.PhoneNumber = phoneToUpdate;

                // Handle address update (prefer nested object over string)
                if (request.AddressObject != null)
                {
                    // Serialize nested address object to JSON string for User table
                    var addressJson = JsonSerializer.Serialize(request.AddressObject);
                    patient.User.Address = addressJson;
                    
                    // Store address as comma-separated string for Patient table
                    var addressParts = new List<string>();
                    if (!string.IsNullOrEmpty(request.AddressObject.Street)) addressParts.Add(request.AddressObject.Street);
                    if (!string.IsNullOrEmpty(request.AddressObject.City)) addressParts.Add(request.AddressObject.City);
                    if (!string.IsNullOrEmpty(request.AddressObject.State)) addressParts.Add(request.AddressObject.State);
                    if (!string.IsNullOrEmpty(request.AddressObject.ZipCode)) addressParts.Add(request.AddressObject.ZipCode);
                    if (!string.IsNullOrEmpty(request.AddressObject.Country)) addressParts.Add(request.AddressObject.Country);
                    
                    patient.Address = addressParts.Any() ? string.Join(", ", addressParts) : null;
                }
                else if (!string.IsNullOrEmpty(request.Address))
                {
                    // Backward compatibility: if string provided, use it for both
                    patient.Address = request.Address;
                    patient.User.Address = request.Address;
                }

                // Handle emergency contact update
                if (request.EmergencyContact != null)
                {
                    // Serialize nested emergency contact object to JSON string for User table
                    var emergencyContactJson = JsonSerializer.Serialize(request.EmergencyContact);
                    patient.User.EmergencyContact = emergencyContactJson;
                    
                    // Update separate fields for Patient table
                    if (!string.IsNullOrEmpty(request.EmergencyContact.Name))
                        patient.EmergencyContactName = request.EmergencyContact.Name;
                    
                    if (!string.IsNullOrEmpty(request.EmergencyContact.Phone))
                    {
                        patient.EmergencyContactPhone = request.EmergencyContact.Phone;
                        patient.User.EmergencyPhone = request.EmergencyContact.Phone;
                    }
                }

                patient.UpdatedAt = DateTime.UtcNow;
                patient.User.UpdatedAt = DateTime.UtcNow;

                // Update User record
                var updateResult = await _userManager.UpdateAsync(patient.User);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(new { 
                        error = "Update Failed", 
                        message = "Failed to update user profile",
                        errors = updateResult.Errors.Select(e => e.Description)
                    });
                }

                // Update Patient record
                await _context.SaveChangesAsync();

                // Return updated patient data
                var response = new PatientResponse
                {
                    Id = patient.Id,
                    UserId = patient.UserId,
                    PatientNumber = patient.PatientNumber,
                    FirstName = patient.User.FirstName,
                    LastName = patient.User.LastName,
                    Phone = patient.User.PhoneNumber ?? "",
                    Email = patient.User.Email ?? "",
                    DateOfBirth = patient.DateOfBirth,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    CreatedAt = patient.CreatedAt,
                    UpdatedAt = patient.UpdatedAt
                };

                return Ok(new { 
                    success = true,
                    message = "Patient updated successfully",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete patients
        public async Task<ActionResult> DeletePatient(Guid id)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                {
                    return NotFound(new { error = "Patient not found" });
                }

                // Soft delete - deactivate the user instead of hard delete
                patient.User.IsActive = false;
                patient.User.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Patient deactivated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting patient: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientSummaryDto>>> SearchPatients([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { error = "Search query is required" });
                }

                var patients = await _context.Patients
                    .Include(p => p.User)
                    .Where(p => p.User.FirstName.Contains(query) ||
                               p.User.LastName.Contains(query) ||
                               p.PatientNumber.Contains(query) ||
                               p.User.PhoneNumber.Contains(query))
                    .Select(p => new PatientSummaryDto
                    {
                        Id = p.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        PatientNumber = p.PatientNumber,
                        Phone = p.User.PhoneNumber,
                        Email = p.User.Email
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}

