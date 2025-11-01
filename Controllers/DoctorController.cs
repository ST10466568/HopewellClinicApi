using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Models;
using HopewellClinicApi.Attributes;
using HopewellClinicApi.Services;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly DoctorScheduleService _scheduleService;

        public DoctorController(HopewellDbContext context, DoctorScheduleService scheduleService)
        {
            _context = context;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Redirect to the correct shift schedule endpoint
        /// </summary>
        [HttpGet("{doctorId}/shifts")]
        [AllowAnonymous]
        public IActionResult GetDoctorShiftsRedirect(Guid doctorId)
        {
            // Redirect to the correct endpoint in BookingController
            return Redirect($"/api/Booking/doctor/{doctorId}/shifts");
        }

        /// <summary>
        /// Get all doctors for admin schedule view
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive && s.User != null && s.User.IsActive)
                    .Select(s => new
                    {
                        id = s.Id,
                        firstName = s.User != null ? s.User.FirstName : "",
                        lastName = s.User != null ? s.User.LastName : "",
                        specialization = "General Practice",
                        isActive = s.IsActive && s.User != null && s.User.IsActive,
                        email = s.User != null ? s.User.Email : "",
                        phone = s.User != null ? s.User.PhoneNumber : "",
                        staffNumber = s.StaffNumber
                    })
                    .OrderBy(d => d.lastName)
                    .ThenBy(d => d.firstName)
                    .ToListAsync();

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("{doctorId}/patients")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PatientSummaryDto>>> GetDoctorPatients(Guid doctorId)
        {
            try
            {
                // Verify doctor exists
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { error = "Doctor not found" });
                }

                // Get patients who have appointments with this doctor
                var patients = await _context.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Where(a => (a.StaffId == doctorId || a.DoctorId == doctorId) && a.Patient != null && a.Patient.User != null)
                    .Select(a => new PatientSummaryDto
                    {
                        Id = a.Patient.Id,
                        FirstName = a.Patient.User != null ? a.Patient.User.FirstName : "",
                        LastName = a.Patient.User != null ? a.Patient.User.LastName : "",
                        PatientNumber = a.Patient.PatientNumber,
                        Phone = a.Patient.User != null ? a.Patient.User.PhoneNumber : "",
                        Email = a.Patient.User != null ? a.Patient.User.Email : ""
                    })
                    .Distinct()
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("{doctorId}/appointments/upcoming")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult<PagedResult<AppointmentResponse>>> GetUpcomingAppointments(
            Guid doctorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                // Authorization: Doctor can only see their own appointments
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("admin");
                
                if (!isAdmin && currentUserId != null && Guid.TryParse(currentUserId, out var userId))
                {
                    var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
                    if (staff == null || staff.Id != doctorId)
                    {
                        return StatusCode(403, new { error = "You can only view your own upcoming appointments" });
                    }
                }

                // Verify doctor exists
                var doctor = await _context.Staff.FindAsync(doctorId);
                if (doctor == null)
                {
                    return NotFound(new { error = "Doctor not found" });
                }

                var now = DateTime.Now;
                var today = DateTime.Today;

                // Build base query for upcoming confirmed appointments
                // Check both StaffId and DoctorId to handle appointments that might have either field set
                var query = _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Where(a => (a.StaffId == doctorId || a.DoctorId == doctorId) && 
                                a.Status == "confirmed" &&
                                // Appointment date is today or future
                                (a.AppointmentDate > today || 
                                 (a.AppointmentDate == today && a.StartTime > TimeOnly.FromDateTime(now))));

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(a => 
                        (a.Patient != null && a.Patient.User != null && 
                         (a.Patient.User.FirstName.ToLower().Contains(searchLower) ||
                          a.Patient.User.LastName.ToLower().Contains(searchLower) ||
                          (a.Patient.User.Email != null && a.Patient.User.Email.ToLower().Contains(searchLower)))) ||
                        (a.Service != null && a.Service.Name.ToLower().Contains(searchLower)));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var appointments = await query
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AppointmentResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        Notes = a.Notes,
                        Service = a.Service != null ? new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes,
                            Price = a.Service.Price,
                            IsActive = a.Service.IsActive,
                            CreatedAt = a.Service.CreatedAt,
                            UpdatedAt = a.Service.UpdatedAt
                        } : null,
                        Patient = a.Patient != null && a.Patient.User != null ? new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? "",
                            Email = a.Patient.User.Email
                        } : null,
                        Staff = a.Staff != null && a.Staff.User != null ? new StaffResponse
                        {
                            Id = a.Staff.Id,
                            UserId = a.Staff.UserId,
                            StaffNumber = a.Staff.StaffNumber,
                            FirstName = a.Staff.User.FirstName,
                            LastName = a.Staff.User.LastName,
                            Role = "doctor",
                            Phone = a.Staff.User.PhoneNumber,
                            IsActive = a.Staff.User.IsActive
                        } : null
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new PagedResult<AppointmentResponse>
                {
                    Items = appointments,
                    TotalCount = totalCount,
                    Page = page,
                    Limit = pageSize,
                    TotalPages = totalPages
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("appointments/walkin")]
        public async Task<ActionResult> CreateWalkinAppointment([FromBody] CreateWalkinAppointmentDto request)
        {
            try
            {
                // Parse time string to TimeOnly
                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                {
                    return BadRequest(new { error = "Invalid start time format. Use HH:mm:ss or HH:mm" });
                }

                // Validate doctor exists
                var doctor = await _context.Staff.FindAsync(request.DoctorId);
                if (doctor == null)
                {
                    return BadRequest(new { error = "Doctor not found" });
                }

                // Validate service exists
                var service = await _context.Services.FindAsync(request.ServiceId);
                if (service == null)
                {
                    return BadRequest(new { error = "Service not found" });
                }

                // Check if patient already exists by phone number
                var existingPatient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.User.PhoneNumber == request.PatientPhone);

                Patient patient;
                if (existingPatient == null)
                {
                    // Create new patient
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = $"{request.PatientFirstName.ToLower()}.{request.PatientLastName.ToLower()}@walkin.local",
                        Email = $"{request.PatientFirstName.ToLower()}.{request.PatientLastName.ToLower()}@walkin.local",
                        FirstName = request.PatientFirstName,
                        LastName = request.PatientLastName,
                        PhoneNumber = request.PatientPhone,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);

                    patient = new Patient
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PatientNumber = $"WALK{DateTime.Now:yyyyMMdd}{user.Id.ToString().Substring(0, 4).ToUpper()}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Patients.Add(patient);
                }
                else
                {
                    patient = existingPatient;
                }

                // Calculate end time based on service duration
                var endTime = startTime.AddMinutes(service.DurationMinutes);

                // Check for time conflicts
                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.AppointmentDate == request.AppointmentDate &&
                               a.StaffId == request.DoctorId &&
                               a.Status != "cancelled" &&
                               ((a.StartTime <= startTime && a.EndTime > startTime) ||
                                (a.StartTime < endTime && a.EndTime >= endTime) ||
                                (a.StartTime >= startTime && a.EndTime <= endTime)))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    return BadRequest(new { error = "Doctor is not available at this time" });
                }

                // Create the walk-in appointment
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patient.Id,
                    ServiceId = request.ServiceId,
                    StaffId = request.DoctorId,
                    AppointmentDate = request.AppointmentDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = "confirmed",
                    BookingType = "walkin",
                    Notes = "Walk-in appointment",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Walk-in appointment created successfully", appointmentId = appointment.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating walk-in appointment: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        // NEW ENDPOINTS FOR DOCTOR DASHBOARD

        // Test endpoint to check if DoctorController works
        [HttpGet("test-simple")]
        [Authorize]
        public ActionResult TestSimple()
        {
            return Ok(new { message = "DoctorController simple test works", timestamp = DateTime.UtcNow });
        }

        [HttpGet("test-db")]
        [Authorize]
        public async Task<ActionResult> TestDatabase()
        {
            try
            {
                var count = await _context.ShiftSchedules.CountAsync();
                return Ok(new { message = "Database connection works", shiftSchedulesCount = count, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Database error", message = ex.Message });
            }
        }

        // Doctor Shift Management - Redirect to BookingController

        [HttpPut("{doctorId}/shifts")]
        [Authorize]
        public async Task<ActionResult> UpdateDoctorShifts(Guid doctorId, [FromBody] UpdateDoctorShiftsRequest request)
        {
            try
            {
                // Validate request
                if (request?.Shifts == null || !request.Shifts.Any())
                {
                    return BadRequest(new { error = "No shifts provided" });
                }

                // Convert DTOs to service DTOs
                var shiftDtos = request.Shifts.Select(s => new DoctorShiftDto
                {
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsActive = s.IsActive
                }).ToList();

                // Use the service layer
                await _scheduleService.UpdateDoctorWeeklyShiftsAsync(doctorId, shiftDtos);

                return Ok(new { message = "Doctor shifts updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "DOCTOR_NOT_FOUND", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "INTERNAL_ERROR", message = "An error occurred while updating the shifts" });
            }
        }

        // Appointment Approval System
        [HttpPut("appointments/{appointmentId}/approve")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult> ApproveAppointment(Guid appointmentId, [FromBody] ApproveAppointmentRequest request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                   User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
                {
                    return Unauthorized(new { error = "User ID not found in token" });
                }

                // Get current doctor's staff record
                var staff = await _context.Staff
                    .FirstOrDefaultAsync(s => s.UserId == userId);
                
                if (staff == null)
                {
                    return StatusCode(403, new { error = "Only doctors can approve appointments" });
                }

                // Get appointment with related data
                var appointment = await _context.Appointments
                    .Include(a => a.Staff)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);
                
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                // Verify doctor owns this appointment (check both StaffId and DoctorId)
                var isAssignedToDoctor = appointment.StaffId == staff.Id || 
                                        (appointment.DoctorId.HasValue && appointment.DoctorId == staff.Id);
                
                if (!isAssignedToDoctor)
                {
                    return StatusCode(403, new { error = "You can only approve appointments assigned to you" });
                }

                // If appointment has DoctorId but not StaffId, update StaffId to match
                if (appointment.DoctorId == staff.Id && appointment.StaffId != staff.Id)
                {
                    appointment.StaffId = staff.Id;
                }

                appointment.ApprovalStatus = ApprovalStatus.Approved;
                appointment.Status = "confirmed";
                appointment.ApprovedAt = DateTime.UtcNow;
                appointment.ApprovedBy = currentUserId;
                if (!string.IsNullOrEmpty(request?.Notes))
                {
                    appointment.Notes = string.IsNullOrEmpty(appointment.Notes) 
                        ? request.Notes 
                        : $"{appointment.Notes}\n{request.Notes}";
                }
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment approved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPut("appointments/{appointmentId}/reject")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult> RejectAppointment(Guid appointmentId, [FromBody] RejectAppointmentRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Reason))
                {
                    return BadRequest(new { error = "Rejection reason is required" });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                   User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
                {
                    return Unauthorized(new { error = "User ID not found in token" });
                }

                // Get current doctor's staff record
                var staff = await _context.Staff
                    .FirstOrDefaultAsync(s => s.UserId == userId);
                
                if (staff == null)
                {
                    return StatusCode(403, new { error = "Only doctors can reject appointments" });
                }

                // Get appointment with related data
                var appointment = await _context.Appointments
                    .Include(a => a.Staff)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);
                
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                // Verify doctor owns this appointment (check both StaffId and DoctorId)
                var isAssignedToDoctor = appointment.StaffId == staff.Id || 
                                        (appointment.DoctorId.HasValue && appointment.DoctorId == staff.Id);
                
                if (!isAssignedToDoctor)
                {
                    return StatusCode(403, new { error = "You can only reject appointments assigned to you" });
                }

                // If appointment has DoctorId but not StaffId, update StaffId to match
                if (appointment.DoctorId == staff.Id && appointment.StaffId != staff.Id)
                {
                    appointment.StaffId = staff.Id;
                }

                appointment.ApprovalStatus = ApprovalStatus.Rejected;
                appointment.Status = "cancelled";
                appointment.RejectionReason = request.Reason;
                appointment.ApprovedAt = DateTime.UtcNow;
                appointment.ApprovedBy = currentUserId;
                appointment.UpdatedAt = DateTime.UtcNow;

                // Add rejection reason to notes if provided
                var rejectionNote = $"Rejected by doctor: {request.Reason}";
                appointment.Notes = string.IsNullOrEmpty(appointment.Notes) 
                    ? rejectionNote 
                    : $"{appointment.Notes}\n{rejectionNote}";

                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment rejected successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        // Enhanced Doctor Appointments
        [HttpGet("{doctorId}/appointments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentWithApprovalResponse>>> GetAllDoctorAppointments(Guid doctorId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Where(a => a.StaffId == doctorId || a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Select(a => new AppointmentWithApprovalResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        ApprovalStatus = a.ApprovalStatus,
                        RejectionReason = a.RejectionReason,
                        ApprovedAt = a.ApprovedAt,
                        ApprovedBy = a.ApprovedBy,
                        Notes = a.Notes,
                        BookingType = a.BookingType,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        },
                        Staff = a.Staff != null && a.Staff.User != null ? new StaffResponse
                        {
                            Id = a.Staff.Id,
                            UserId = a.Staff.UserId,
                            StaffNumber = a.Staff.StaffNumber,
                            FirstName = a.Staff.User.FirstName,
                            LastName = a.Staff.User.LastName,
                            Role = "staff",
                            Phone = a.Staff.User.PhoneNumber,
                            IsActive = a.Staff.User.IsActive
                        } : null
                    })
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("{doctorId}/schedule")]
        [Authorize]
        public async Task<ActionResult<DoctorScheduleResponse>> GetDoctorSchedule(Guid doctorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Where(a => (a.StaffId == doctorId || a.DoctorId == doctorId) && 
                               a.AppointmentDate >= startDate && 
                               a.AppointmentDate <= endDate)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Select(a => new AppointmentWithApprovalResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        ApprovalStatus = a.ApprovalStatus,
                        RejectionReason = a.RejectionReason,
                        ApprovedAt = a.ApprovedAt,
                        ApprovedBy = a.ApprovedBy,
                        Notes = a.Notes,
                        BookingType = a.BookingType,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        },
                        Staff = a.Staff != null && a.Staff.User != null ? new StaffResponse
                        {
                            Id = a.Staff.Id,
                            UserId = a.Staff.UserId,
                            StaffNumber = a.Staff.StaffNumber,
                            FirstName = a.Staff.User.FirstName,
                            LastName = a.Staff.User.LastName,
                            Role = "staff",
                            Phone = a.Staff.User.PhoneNumber,
                            IsActive = a.Staff.User.IsActive
                        } : null
                    })
                    .ToListAsync();

                var shiftSchedules = await _context.ShiftSchedules
                    .Where(ds => ds.DoctorId == doctorId && ds.IsActive)
                    .ToListAsync();

                var shifts = shiftSchedules.Select(ds => new
                {
                    id = ds.Id,
                    dayOfWeek = ds.DayOfWeek,
                    startTime = ds.StartTime.ToString(@"hh\:mm"),
                    endTime = ds.EndTime.ToString(@"hh\:mm"),
                    isActive = ds.IsActive,
                    doctorId = ds.DoctorId
                }).ToList();

                var response = new DoctorScheduleResponse
                {
                    Date = startDate,
                    Appointments = appointments,
                    Shifts = shifts.Cast<object>().ToList()
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Patient Details for Doctors
        [HttpGet("patients/{patientId}")]
        public async Task<ActionResult<PatientDetailsForDoctorResponse>> GetPatientDetailsForDoctor(Guid patientId)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Service)
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Staff)
                            .ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(p => p.Id == patientId);

                if (patient == null)
                {
                    return NotFound(new { error = "Patient not found" });
                }

                var appointmentHistory = patient.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new AppointmentWithApprovalResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        ApprovalStatus = a.ApprovalStatus,
                        RejectionReason = a.RejectionReason,
                        ApprovedAt = a.ApprovedAt,
                        ApprovedBy = a.ApprovedBy,
                        Notes = a.Notes,
                        BookingType = a.BookingType,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        },
                        Staff = a.Staff != null && a.Staff.User != null ? new StaffResponse
                        {
                            Id = a.Staff.Id,
                            UserId = a.Staff.UserId,
                            StaffNumber = a.Staff.StaffNumber,
                            FirstName = a.Staff.User.FirstName,
                            LastName = a.Staff.User.LastName,
                            Role = "staff",
                            Phone = a.Staff.User.PhoneNumber,
                            IsActive = a.Staff.User.IsActive
                        } : null
                    })
                    .ToList();

                var response = new PatientDetailsForDoctorResponse
                {
                    Id = patient.Id,
                    PatientNumber = patient.PatientNumber,
                    FirstName = patient.User.FirstName,
                    LastName = patient.User.LastName,
                    Email = patient.User.Email ?? "",
                    Phone = patient.User.PhoneNumber ?? "",
                    DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    CreatedAt = patient.CreatedAt,
                    UpdatedAt = patient.UpdatedAt,
                    AppointmentHistory = appointmentHistory
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private int GetDayOfWeekNumber(string dayOfWeek)
        {
            return dayOfWeek.ToLower() switch
            {
                "sunday" => 0,
                "monday" => 1,
                "tuesday" => 2,
                "wednesday" => 3,
                "thursday" => 4,
                "friday" => 5,
                "saturday" => 6,
                _ => 0
            };
        }
    }
}
