using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Attributes;
using HopewellClinicApi.Services;
using System.Security.Claims;

namespace HopewellClinicApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly IAppointmentManagementService _appointmentManagementService;
        private readonly IAppointmentStatusService _appointmentStatusService;

        public AppointmentsController(HopewellDbContext context, IAppointmentManagementService appointmentManagementService, IAppointmentStatusService appointmentStatusService)
        {
            _context = context;
            _appointmentManagementService = appointmentManagementService;
            _appointmentStatusService = appointmentStatusService;
        }

        /// <summary>
        /// Create a test appointment for testing purposes
        /// </summary>
        /// <summary>
        /// Minimal test endpoint to isolate the 500 error issue
        /// </summary>
        [HttpPut("test-minimal-update/{id}")]
        [JwtAuthorize]
        public async Task<ActionResult> TestMinimalUpdate(Guid id, [FromBody] object request)
        {
            try
            {
                return Ok(new { 
                    success = true, 
                    message = "Minimal test successful",
                    id = id,
                    requestType = request?.GetType().Name ?? "null",
                    userAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    userId = User.FindFirst("sub")?.Value ?? "not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Minimal test failed", 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("test/create")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CreateTestAppointment()
        {
            try
            {
                // Get first available service
                var service = await _context.Services.FirstOrDefaultAsync();
                if (service == null)
                {
                    return BadRequest(new { error = "No services available" });
                }

                // Get first available patient
                var patient = await _context.Patients.FirstOrDefaultAsync();
                if (patient == null)
                {
                    return BadRequest(new { error = "No patients available" });
                }

                // Create test appointment
                var testAppointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.UtcNow.AddDays(1),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 0),
                    Status = "Scheduled",
                    Notes = "Test appointment for API testing",
                    ServiceId = service.Id,
                    PatientId = patient.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(testAppointment);
                await _context.SaveChangesAsync();

                return Ok(new {
                    success = true,
                    message = "Test appointment created successfully",
                    appointmentId = testAppointment.Id,
                    appointmentDate = testAppointment.AppointmentDate,
                    startTime = testAppointment.StartTime,
                    endTime = testAppointment.EndTime,
                    status = testAppointment.Status,
                    serviceName = service.Name,
                    patientName = $"{patient.User.FirstName} {patient.User.LastName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Failed to create test appointment", 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Simple database test endpoint
        /// </summary>
        [HttpGet("test/database")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestDatabase()
        {
            try
            {
                // Test basic database connection
                var appointmentCount = await _context.Appointments.CountAsync();
                var serviceCount = await _context.Services.CountAsync();
                var patientCount = await _context.Patients.CountAsync();
                
                // Test if we can find any appointment
                var firstAppointment = await _context.Appointments.FirstOrDefaultAsync();
                
                return Ok(new {
                    message = "Database test successful",
                    timestamp = DateTime.UtcNow,
                    appointmentCount = appointmentCount,
                    serviceCount = serviceCount,
                    patientCount = patientCount,
                    firstAppointmentId = firstAppointment?.Id,
                    firstAppointmentDate = firstAppointment?.AppointmentDate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Database test failed", 
                    message = ex.Message, 
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Simple test endpoint to check service availability
        /// </summary>
        [HttpGet("test/service")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestService()
        {
            try
            {
                // Test if service is injected
                var serviceAvailable = _appointmentManagementService != null;
                
                // Test database connection
                var appointmentCount = await _context.Appointments.CountAsync();
                
                // Test service method with minimal data
                var testResult = await _appointmentManagementService.ValidateAppointmentUpdateAsync(
                    Guid.NewGuid(), 
                    new AdminUpdateAppointmentRequest 
                    { 
                        AppointmentDate = DateTime.UtcNow.AddDays(1),
                        StartTime = "10:00",
                        EndTime = "11:00",
                        Status = "Confirmed",
                        ServiceId = Guid.NewGuid()
                    });

                return Ok(new {
                    message = "Service test successful",
                    timestamp = DateTime.UtcNow,
                    serviceAvailable = serviceAvailable,
                    appointmentCount = appointmentCount,
                    validationTest = testResult.IsValid ? "Service working" : $"Service validation failed: {string.Join(", ", testResult.Errors)}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Service test failed", 
                    message = ex.Message, 
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Get appointments for a specific doctor on a specific date (Anonymous version for frontend)
        /// </summary>
        [HttpGet("doctor/{doctorId}/date/{date}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctorAppointmentsByDate(string doctorId, string date)
        {
            try
            {
                if (string.IsNullOrEmpty(doctorId))
                {
                    return BadRequest(new { error = "Doctor ID is required" });
                }

                if (string.IsNullOrEmpty(date))
                {
                    return BadRequest(new { error = "Date is required" });
                }

                if (!Guid.TryParse(doctorId, out var doctorGuid))
                {
                    return BadRequest(new { error = "Invalid doctor ID format" });
                }

                if (!DateTime.TryParse(date, out var appointmentDate))
                {
                    return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD" });
                }

                // Verify doctor exists
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorGuid);

                if (doctor == null)
                {
                    return NotFound(new { error = "Doctor not found" });
                }

                var appointments = await _context.Appointments
                    .Where(a => (a.StaffId == doctorGuid || a.DoctorId == doctorGuid) && 
                               a.AppointmentDate == appointmentDate.Date &&
                               (a.Status == "pending" || a.Status == "confirmed" || a.Status == "approved" || a.Status == "scheduled"))
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .OrderBy(a => a.StartTime)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    notes = a.Notes,
                    createdAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    staffId = a.StaffId,
                    doctorId = a.DoctorId,
                    patient = new
                    {
                        id = a.Patient.Id,
                        firstName = a.Patient.User.FirstName,
                        lastName = a.Patient.User.LastName,
                        email = a.Patient.User.Email,
                        phone = a.Patient.User.PhoneNumber
                    },
                    service = new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name,
                        durationMinutes = a.Service.DurationMinutes,
                        price = a.Service.Price
                    },
                    staff = a.Staff != null ? new
                    {
                        id = a.Staff.Id,
                        staffId = a.Staff.Id,
                        userId = a.Staff.UserId,
                        firstName = a.Staff.User.FirstName,
                        lastName = a.Staff.User.LastName,
                        email = a.Staff.User.Email,
                        role = "doctor"
                    } : null
                }).ToList();

                // Return the format expected by frontend
                return Ok(new
                {
                    appointments = appointmentResults,
                    totalAppointmentsFound = appointments.Count,
                    doctorId = doctorGuid,
                    requestedDate = appointmentDate.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint to check if doctor exists and has any appointments
        /// </summary>
        [HttpGet("debug-doctor/{doctorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> DebugDoctorAppointments(string doctorId)
        {
            try
            {
                if (!Guid.TryParse(doctorId, out var doctorGuid))
                {
                    return BadRequest(new { error = "Invalid doctor ID format" });
                }

                // Check if doctor exists
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorGuid);

                if (doctor == null)
                {
                    return NotFound(new { error = "Doctor not found", doctorId = doctorGuid });
                }

                // Get all appointments for this doctor
                var allAppointments = await _context.Appointments
                    .Where(a => a.StaffId == doctorGuid || a.DoctorId == doctorGuid)
                    .Select(a => new
                    {
                        id = a.Id,
                        appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                        startTime = a.StartTime.ToString("HH:mm"),
                        endTime = a.EndTime.ToString("HH:mm"),
                        status = a.Status,
                        staffId = a.StaffId,
                        doctorId = a.DoctorId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    doctor = new
                    {
                        id = doctor.Id,
                        firstName = doctor.User.FirstName,
                        lastName = doctor.User.LastName,
                        isActive = doctor.IsActive
                    },
                    totalAppointments = allAppointments.Count,
                    appointments = allAppointments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Get appointments for a specific doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctorAppointments(string doctorId)
        {
            try
            {
                if (string.IsNullOrEmpty(doctorId))
                {
                    return BadRequest("Doctor ID is required");
                }

                if (!Guid.TryParse(doctorId, out var doctorGuid))
                {
                    return BadRequest("Invalid doctor ID format");
                }

                var appointments = await _context.Appointments
                    .Where(a => (a.StaffId == doctorGuid || a.DoctorId == doctorGuid))
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm:ss"),
                    endTime = a.EndTime.ToString("HH:mm:ss"),
                    status = a.Status,
                    notes = a.Notes,
                    patient = new
                    {
                        id = a.Patient.Id,
                        firstName = a.Patient.User.FirstName,
                        lastName = a.Patient.User.LastName,
                        email = a.Patient.User.Email,
                        phone = a.Patient.User.PhoneNumber,
                        dateOfBirth = a.Patient.DateOfBirth.HasValue ? a.Patient.DateOfBirth.Value.ToString("yyyy-MM-dd") : null
                    },
                    service = new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name,
                        durationMinutes = a.Service.DurationMinutes
                    }
                }).ToList();

                return Ok(appointmentResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Get appointments with pagination and search (Enhanced version)
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> SearchAppointments(
            [FromQuery] string? doctorId = null,
            [FromQuery] string? date = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .AsQueryable();

                // Filter by doctor
                if (!string.IsNullOrEmpty(doctorId) && Guid.TryParse(doctorId, out var doctorGuid))
                {
                    query = query.Where(a => a.StaffId == doctorGuid || a.DoctorId == doctorGuid);
                }

                // Filter by date
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var appointmentDate))
                {
                    query = query.Where(a => a.AppointmentDate == appointmentDate.Date);
                }

                // Filter by status
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Status == status);
                }

                // Search functionality
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(a => 
                        a.Patient.User.FirstName.Contains(search) ||
                        a.Patient.User.LastName.Contains(search) ||
                        a.Service.Name.Contains(search) ||
                        a.Notes.Contains(search));
                }

                // Get total count for pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var appointments = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    notes = a.Notes,
                    createdAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    staffId = a.StaffId,
                    doctorId = a.DoctorId,
                    patient = new
                    {
                        id = a.Patient.Id,
                        firstName = a.Patient.User.FirstName,
                        lastName = a.Patient.User.LastName,
                        email = a.Patient.User.Email,
                        phone = a.Patient.User.PhoneNumber
                    },
                    service = new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name,
                        durationMinutes = a.Service.DurationMinutes,
                        price = a.Service.Price
                    },
                    staff = a.Staff != null ? new
                    {
                        id = a.Staff.Id,
                        staffId = a.Staff.Id,
                        userId = a.Staff.UserId,
                        firstName = a.Staff.User.FirstName,
                        lastName = a.Staff.User.LastName,
                        email = a.Staff.User.Email,
                        role = "doctor"
                    } : null
                }).ToList();

                return Ok(new
                {
                    appointments = appointmentResults,
                    pagination = new
                    {
                        page = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                        hasNextPage = page * pageSize < totalCount,
                        hasPreviousPage = page > 1
                    },
                    filters = new
                    {
                        doctorId = doctorId,
                        date = date,
                        status = status,
                        search = search
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Get all appointments (Anonymous version for frontend fallback)
        /// </summary>
        /// <summary>
        /// Get all appointments - Frontend compatible endpoint
        /// </summary>
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAppointmentsFrontend()
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    notes = a.Notes,
                    createdAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    staffId = a.StaffId,
                    doctorId = a.DoctorId,
                    patientId = a.PatientId,
                    patient = new
                    {
                        id = a.Patient?.Id,
                        firstName = a.Patient?.User?.FirstName,
                        lastName = a.Patient?.User?.LastName,
                        email = a.Patient?.User?.Email,
                        phone = a.Patient?.User?.PhoneNumber
                    },
                    service = new
                    {
                        id = a.Service?.Id,
                        name = a.Service?.Name,
                        durationMinutes = a.Service?.DurationMinutes,
                        price = a.Service?.Price
                    },
                    staff = a.Staff != null ? new
                    {
                        id = a.Staff.Id,
                        staffId = a.Staff.Id,
                        userId = a.Staff.UserId,
                        firstName = a.Staff.User.FirstName,
                        lastName = a.Staff.User.LastName,
                        role = "staff",
                        phone = a.Staff.User.PhoneNumber,
                        isActive = a.Staff.User.IsActive
                    } : null
                }).ToList();

                return Ok(appointmentResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("all-appointments")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    notes = a.Notes,
                    createdAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    staffId = a.StaffId,
                    doctorId = a.DoctorId,
                    patientId = a.PatientId,
                    patient = new
                    {
                        id = a.Patient?.Id,
                        firstName = a.Patient?.User?.FirstName,
                        lastName = a.Patient?.User?.LastName,
                        email = a.Patient?.User?.Email,
                        phone = a.Patient?.User?.PhoneNumber
                    },
                    service = new
                    {
                        id = a.Service?.Id,
                        name = a.Service?.Name,
                        durationMinutes = a.Service?.DurationMinutes,
                        price = a.Service?.Price
                    },
                    staff = a.Staff != null ? new
                    {
                        id = a.Staff.Id,
                        staffId = a.Staff.Id,
                        userId = a.Staff.UserId,
                        firstName = a.Staff.User.FirstName,
                        lastName = a.Staff.User.LastName,
                        email = a.Staff.User.Email,
                        role = "doctor"
                    } : null
                }).ToList();

                return Ok(appointmentResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("list")]
        [JwtAuthorize]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Select(a => new AppointmentResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        Notes = a.Notes,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = a.Patient != null ? new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        } : null,
                        Staff = a.Staff != null ? new StaffResponse
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

        [HttpGet("get/{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetAppointment(Guid id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                var response = new AppointmentResponse
                {
                    Id = appointment.Id,
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,
                    Status = appointment.Status,
                    Notes = appointment.Notes,
                    Service = new ServiceResponse
                    {
                        Id = appointment.Service.Id,
                        Name = appointment.Service.Name,
                        Description = appointment.Service.Description,
                        DurationMinutes = appointment.Service.DurationMinutes
                    },
                    Patient = appointment.Patient != null ? new PatientResponse
                    {
                        Id = appointment.Patient.Id,
                        FirstName = appointment.Patient.User.FirstName,
                        LastName = appointment.Patient.User.LastName,
                        Phone = appointment.Patient.User.PhoneNumber ?? ""
                    } : null,
                    Staff = appointment.Staff != null ? new StaffResponse
                    {
                        Id = appointment.Staff.Id,
                        UserId = appointment.Staff.UserId,
                        StaffNumber = appointment.Staff.StaffNumber,
                        FirstName = appointment.Staff.User.FirstName,
                        LastName = appointment.Staff.User.LastName,
                        Role = "staff",
                        Phone = appointment.Staff.User.PhoneNumber,
                        IsActive = appointment.Staff.User.IsActive
                    } : null
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all appointments - ANONYMOUS VERSION FOR FRONTEND
        /// This is the route the frontend expects: GET /api/Appointments
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAppointmentsAnonymous()
        {
            try
            {
                Console.WriteLine($"GET /api/Appointments - Starting method execution (Anonymous)");
                
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var appointmentResults = appointments.Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    notes = a.Notes,
                    createdAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    staffId = a.StaffId,
                    doctorId = a.DoctorId,
                    patientId = a.PatientId,
                    patient = a.Patient != null ? new
                    {
                        id = a.Patient.Id,
                        firstName = a.Patient.User.FirstName,
                        lastName = a.Patient.User.LastName,
                        email = a.Patient.User.Email,
                        phone = a.Patient.User.PhoneNumber
                    } : null,
                    service = a.Service != null ? new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name,
                        durationMinutes = a.Service.DurationMinutes,
                        price = a.Service.Price
                    } : null,
                    staff = a.Staff != null ? new
                    {
                        id = a.Staff.Id,
                        staffId = a.Staff.Id,
                        userId = a.Staff.UserId,
                        firstName = a.Staff.User.FirstName,
                        lastName = a.Staff.User.LastName,
                        email = a.Staff.User.Email,
                        role = "doctor"
                    } : null
                }).ToList();

                Console.WriteLine($"GET /api/Appointments - Found {appointmentResults.Count} appointments");
                return Ok(appointmentResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET /api/Appointments Error: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPut("admin/update/{id}")]
        public async Task<ActionResult<AppointmentResponse>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                // Update appointment fields
                if (request.AppointmentDate.HasValue)
                    appointment.AppointmentDate = request.AppointmentDate.Value;
                if (request.StartTime.HasValue)
                    appointment.StartTime = request.StartTime.Value;
                if (request.EndTime.HasValue)
                    appointment.EndTime = request.EndTime.Value;
                if (request.Notes != null)
                    appointment.Notes = request.Notes;
                if (request.Status != null)
                    appointment.Status = request.Status;

                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelAppointment(Guid id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                appointment.Status = "cancelled";
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment cancelled successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("{id}/assign-staff")]
        public async Task<ActionResult> AssignStaff(Guid id, [FromBody] AssignStaffRequest request)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                var staff = await _context.Staff.FindAsync(request.StaffId);
                if (staff == null)
                {
                    return NotFound(new { error = "Staff member not found" });
                }

                appointment.StaffId = request.StaffId;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Staff assigned successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<TimeSlotResponse>>> GetAvailableSlots(
            [FromQuery] DateTime date,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                // Get all active time slots
                var allTimeSlots = await _context.TimeSlots
                    .Where(t => t.IsActive)
                    .ToListAsync();

                // Get existing confirmed appointments for that date
                var existingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate == date && a.Status == "confirmed")
                    .ToListAsync();

                // Filter out booked slots
                var availableSlots = allTimeSlots
                    .Where(slot => !existingAppointments.Any(apt => apt.StartTime == slot.StartTime))
                    .Select(slot => new TimeSlotResponse
                    {
                        Id = slot.Id,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        DayOfWeek = slot.DayOfWeek,
                        IsActive = slot.IsActive,
                        CreatedAt = slot.CreatedAt
                    })
                    .OrderBy(s => s.StartTime)
                    .ToList();

                return Ok(availableSlots);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available slots: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointmentsByPatient(Guid patientId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == patientId)
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Select(a => new AppointmentResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        Notes = a.Notes,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = a.Patient != null ? new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        } : null,
                        Staff = a.Staff != null ? new StaffResponse
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

        /// <summary>
        /// Update appointment - Frontend compatible endpoint
        /// </summary>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateAppointmentFrontend(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                // Update appointment fields
                if (request.AppointmentDate.HasValue)
                    appointment.AppointmentDate = request.AppointmentDate.Value;
                if (request.StartTime.HasValue)
                    appointment.StartTime = request.StartTime.Value;
                if (request.EndTime.HasValue)
                    appointment.EndTime = request.EndTime.Value;
                if (!string.IsNullOrEmpty(request.Status))
                    appointment.Status = request.Status;
                if (request.Notes != null)
                    appointment.Notes = request.Notes;

                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Appointment updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete appointment - Frontend compatible endpoint
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteAppointmentFrontend(Guid id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Appointment deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost]
        [JwtAuthorize]
        public async Task<ActionResult<AppointmentResponse>> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                // Parse time strings to TimeOnly
                if (!TimeOnly.TryParse(request.StartTime, out var startTime))
                {
                    return BadRequest(new { error = "Invalid start time format. Use HH:mm:ss or HH:mm" });
                }

                TimeOnly endTime;
                if (string.IsNullOrEmpty(request.EndTime))
                {
                    // Calculate end time based on service duration
                    var service = await _context.Services
                        .FirstOrDefaultAsync(s => s.Id == request.ServiceId);
                    if (service == null)
                    {
                        return BadRequest(new { error = "Service not found" });
                    }
                    endTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                }
                else
                {
                    if (!TimeOnly.TryParse(request.EndTime, out endTime))
                    {
                        return BadRequest(new { error = "Invalid end time format. Use HH:mm:ss or HH:mm" });
                    }
                }

                // Validate patient exists
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId);
                if (patient == null)
                {
                    return BadRequest(new { error = "Patient not found" });
                }

                // Service validation already done above when calculating end time

                // Handle doctor assignment
                Guid? assignedStaffId = request.StaffId;
                
                // If no doctor is specified, assign one automatically
                if (assignedStaffId == null)
                {
                    // Get available doctors for the appointment date
                    var availableDoctors = await _context.Staff
                        .Include(s => s.User)
                        .Where(s => s.IsActive && s.User.IsActive)
                        .ToListAsync();

                    if (availableDoctors.Any())
                    {
                        // Use round-robin assignment based on appointment date and time
                        var appointmentHash = request.AppointmentDate.GetHashCode() + startTime.GetHashCode();
                        var doctorIndex = Math.Abs(appointmentHash) % availableDoctors.Count;
                        assignedStaffId = availableDoctors[doctorIndex].Id;
                    }
                    else
                    {
                        return BadRequest(new { error = "No available doctors found" });
                    }
                }
                else
                {
                    // Validate the specified doctor exists and is active
                    var doctor = await _context.Staff
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == assignedStaffId && s.IsActive && s.User.IsActive);
                    if (doctor == null)
                    {
                        return BadRequest(new { error = "Specified doctor not found or not active" });
                    }
                }

                // Check if the time slot is available for the assigned doctor
                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.AppointmentDate == request.AppointmentDate &&
                               a.StaffId == assignedStaffId &&
                               a.Status != "cancelled" &&
                               ((a.StartTime <= startTime && a.EndTime > startTime) ||
                                (a.StartTime < endTime && a.EndTime >= endTime) ||
                                (a.StartTime >= startTime && a.EndTime <= endTime)))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    return BadRequest(new { error = "Doctor is not available at this time" });
                }

                // Create the appointment with ALL required fields
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(), // ✅ Add the missing Id field
                    PatientId = request.PatientId,
                    ServiceId = request.ServiceId,
                    StaffId = assignedStaffId, // ✅ Assign the doctor
                    AppointmentDate = request.AppointmentDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Notes = request.Notes,
                    Status = "pending",
                    BookingType = "online", // ✅ Add the missing BookingType field
                    CreatedAt = DateTime.UtcNow, // ✅ Add the missing CreatedAt field
                    UpdatedAt = DateTime.UtcNow  // ✅ Add the missing UpdatedAt field
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Return the created appointment with proper includes
                var createdAppointment = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User) // ✅ Include User for Patient
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User) // ✅ Include User for Staff
                    .FirstOrDefaultAsync(a => a.Id == appointment.Id);

                if (createdAppointment == null)
                {
                    return StatusCode(500, new { error = "Failed to retrieve created appointment" });
                }

                var response = new AppointmentResponse
                {
                    Id = createdAppointment.Id,
                    AppointmentDate = createdAppointment.AppointmentDate,
                    StartTime = createdAppointment.StartTime,
                    EndTime = createdAppointment.EndTime,
                    Status = createdAppointment.Status,
                    Notes = createdAppointment.Notes,
                    Service = new ServiceResponse
                    {
                        Id = createdAppointment.Service.Id,
                        Name = createdAppointment.Service.Name,
                        Description = createdAppointment.Service.Description,
                        DurationMinutes = createdAppointment.Service.DurationMinutes,
                        IsActive = createdAppointment.Service.IsActive,
                        CreatedAt = createdAppointment.Service.CreatedAt,
                        UpdatedAt = createdAppointment.Service.UpdatedAt
                    },
                    Patient = new PatientResponse
                    {
                        Id = createdAppointment.Patient.Id,
                        FirstName = createdAppointment.Patient.User.FirstName,
                        LastName = createdAppointment.Patient.User.LastName,
                        Phone = createdAppointment.Patient.User.PhoneNumber ?? ""
                    },
                    Staff = createdAppointment.Staff != null ? new StaffResponse
                    {
                        Id = createdAppointment.Staff.Id,
                        UserId = createdAppointment.Staff.UserId,
                        StaffNumber = createdAppointment.Staff.StaffNumber,
                        FirstName = createdAppointment.Staff.User.FirstName,
                        LastName = createdAppointment.Staff.User.LastName,
                        Role = "doctor",
                        Phone = createdAppointment.Staff.User.PhoneNumber,
                        IsActive = createdAppointment.Staff.User.IsActive
                    } : null
                };

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, response);
            }
            catch (Exception ex)
            {
                // ✅ Better error logging
                Console.WriteLine($"Error creating appointment: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetTodaysAppointments()
        {
            try
            {
                var today = DateTime.Today;
                var appointments = await _context.Appointments
                    .Where(a => a.AppointmentDate == today)
                    .Include(a => a.Service)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Select(a => new AppointmentResponse
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        Notes = a.Notes,
                        Service = new ServiceResponse
                        {
                            Id = a.Service.Id,
                            Name = a.Service.Name,
                            Description = a.Service.Description,
                            DurationMinutes = a.Service.DurationMinutes
                        },
                        Patient = a.Patient != null ? new PatientResponse
                        {
                            Id = a.Patient.Id,
                            FirstName = a.Patient.User.FirstName,
                            LastName = a.Patient.User.LastName,
                            Phone = a.Patient.User.PhoneNumber ?? ""
                        } : null,
                        Staff = a.Staff != null ? new StaffResponse
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
                    .OrderBy(a => a.StartTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // NURSE DASHBOARD ENDPOINTS

        [HttpPost("walkin")]
        [AuthorizeNurse]
        public async Task<ActionResult<WalkInAppointmentResponse>> CreateWalkInAppointment([FromBody] WalkInAppointmentDto dto)
        {
            try
            {
                // Validate service exists
                var service = await _context.Services.FindAsync(dto.ServiceId);
                if (service == null)
                {
                    return BadRequest(new { error = "Service not found" });
                }

                // Validate doctor exists
                var doctor = await _context.Staff.FindAsync(dto.DoctorId);
                if (doctor == null)
                {
                    return BadRequest(new { error = "Doctor not found" });
                }

                // Check if patient already exists by email
                var existingPatient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.User.Email == dto.PatientEmail);

                Patient patient;
                if (existingPatient == null)
                {
                    // Create new patient
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = dto.PatientEmail,
                        Email = dto.PatientEmail,
                        FirstName = dto.PatientFirstName,
                        LastName = dto.PatientLastName,
                        PhoneNumber = dto.PatientPhone,
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

                // Auto-assign current date and time
                var now = DateTime.Now;
                var appointmentDate = now.Date;
                var startTime = new TimeOnly(now.Hour, now.Minute);
                var endTime = startTime.AddMinutes(service.DurationMinutes);

                // Check for time conflicts
                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.AppointmentDate == appointmentDate &&
                               a.StaffId == dto.DoctorId &&
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
                    ServiceId = dto.ServiceId,
                    StaffId = dto.DoctorId,
                    AppointmentDate = appointmentDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = "walkin",
                    BookingType = "walkin",
                    IsWalkIn = true,
                    ServicePrice = service.Price,
                    PaymentStatus = "pending",
                    Notes = dto.Notes,
                    ApprovalStatus = ApprovalStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(new WalkInAppointmentResponse
                {
                    AppointmentId = appointment.Id,
                    Message = "Walk-in appointment created successfully",
                    PatientId = patient.Id,
                    PatientNumber = patient.PatientNumber
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating walk-in appointment: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("{id}/approve-for-doctor")]
        [AuthorizeNurse]
        public async Task<ActionResult<NurseApprovalResponse>> ApproveForDoctor(Guid id, [FromBody] DoctorApprovalDto dto)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                // Validate doctor exists
                var doctor = await _context.Staff.FindAsync(dto.DoctorId);
                if (doctor == null)
                {
                    return BadRequest(new { error = "Doctor not found" });
                }

                // Check for time conflicts with the assigned doctor
                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.Id != id &&
                               a.AppointmentDate == appointment.AppointmentDate &&
                               a.StaffId == dto.DoctorId &&
                               a.Status != "cancelled" &&
                               ((a.StartTime <= appointment.StartTime && a.EndTime > appointment.StartTime) ||
                                (a.StartTime < appointment.EndTime && a.EndTime >= appointment.EndTime) ||
                                (a.StartTime >= appointment.StartTime && a.EndTime <= appointment.EndTime)))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    return BadRequest(new { error = "Doctor is not available at this time" });
                }

                // Update appointment with nurse approval
                appointment.StaffId = dto.DoctorId;
                appointment.Status = "confirmed";
                appointment.ApprovalStatus = ApprovalStatus.Approved;
                appointment.ApprovedByNurseId = User.Identity?.Name; // Get current nurse ID
                appointment.NurseApprovalDate = DateTime.UtcNow;
                appointment.ApprovedAt = DateTime.UtcNow;
                appointment.ApprovedBy = dto.DoctorId.ToString();
                if (!string.IsNullOrEmpty(dto.ApprovalNotes))
                {
                    appointment.ApprovalNotes = dto.ApprovalNotes;
                }
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new NurseApprovalResponse
                {
                    Message = "Appointment approved and assigned to doctor successfully",
                    AppointmentId = appointment.Id,
                    Status = appointment.Status
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get available time slots by doctor - Frontend compatible endpoint
        /// </summary>
        [HttpGet("available-slots-by-doctor")]
        public async Task<ActionResult<object>> GetAvailableSlotsByDoctor(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime date,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                // Use the BookingService to get slots from doctor schedule
                var bookingService = HttpContext.RequestServices.GetRequiredService<HopewellClinicApi.Services.BookingService>();
                var response = await bookingService.GetAvailableSlotsByDoctorAsync(doctorId, date, serviceId);
                
                return Ok(new
                {
                    doctorId = response.DoctorId,
                    date = response.Date,
                    slots = response.AvailableSlots.Select(s => new
                    {
                        id = s.Id,
                        startTime = s.StartTime.ToString(@"hh\:mm\:ss"),
                        endTime = s.EndTime.ToString(@"hh\:mm\:ss"),
                        duration = s.Duration,
                        isAvailable = s.IsAvailable,
                        doctorId = s.DoctorId
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Enhanced endpoint: Get available time slots with doctor filtering
        /// </summary>
        [HttpGet("available-slots-enhanced")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAvailableSlots(
            [FromQuery] DateTime date,
            [FromQuery] Guid? doctorId = null,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var query = _context.TimeSlots
                    .Where(ts => ts.IsActive);

                if (doctorId.HasValue)
                {
                    query = query.Where(ts => ts.DoctorId == doctorId.Value);
                }

                if (date != default)
                {
                    query = query.Where(ts => ts.Date == date.Date);
                }

                var slots = await query
                    .Select(ts => new
                    {
                        ts.Id,
                        ts.StartTime,
                        ts.EndTime,
                        ts.Duration,
                        ts.IsAvailable,
                        ts.DoctorId,
                        Doctor = ts.Doctor != null ? new
                        {
                            ts.Doctor.Id,
                            FirstName = ts.Doctor.User.FirstName,
                            LastName = ts.Doctor.User.LastName,
                            Specialty = "General Practice"
                        } : null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Date = date,
                    DoctorId = doctorId,
                    ServiceId = serviceId,
                    Slots = slots
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Enhanced endpoint: Get doctors on duty for a specific date
        /// </summary>
        [HttpGet("doctors-on-duty")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDoctorsOnDuty([FromQuery] DateTime date, [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                var doctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .Join(_context.ShiftSchedules,
                        s => s.Id,
                        ds => ds.DoctorId,
                        (s, ds) => new { Staff = s, Schedule = ds })
                    .Where(x => x.Schedule.DayOfWeek == dayOfWeek && x.Schedule.IsActive)
                    .Select(x => new
                    {
                        x.Staff.Id,
                        FirstName = x.Staff.User.FirstName,
                        LastName = x.Staff.User.LastName,
                        Specialty = "General Practice",
                        x.Schedule.StartTime,
                        x.Schedule.EndTime,
                        IsAvailable = true
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Date = date,
                    ServiceId = serviceId,
                    Doctors = doctors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing appointment (Admin only) - Backward compatible endpoint
        /// </summary>
        [HttpPut("admin/appointment/{id}")]
        [Authorize]
        public async Task<ActionResult<AppointmentOperationResponse>> UpdateAppointment(Guid id, [FromBody] AdminUpdateAppointmentRequest request)
        {
            try
            {
                // Get current user ID for audit logging
                var currentUserId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || !Guid.TryParse(currentUserId, out var adminId))
                {
                    return Unauthorized(new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = "Unauthorized access"
                    });
                }

                // Validate the request
                var validationResult = await _appointmentManagementService.ValidateAppointmentUpdateAsync(id, request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = string.Join(", ", validationResult.Errors)
                    });
                }

                // Update the appointment
                var result = await _appointmentManagementService.UpdateAppointmentAsync(id, request, adminId);
                
                if (!result.Success)
                {
                    return result.Error == "Appointment not found" 
                        ? NotFound(result) 
                        : StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppointmentOperationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Update an existing appointment (Admin only) - New admin endpoint
        /// </summary>
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AppointmentOperationResponse>> AdminUpdateAppointment(Guid id, [FromBody] AdminUpdateAppointmentRequest request)
        {
            try
            {
                // Get current user ID for audit logging
                var currentUserId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || !Guid.TryParse(currentUserId, out var adminId))
                {
                    return Unauthorized(new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = "Unauthorized access"
                    });
                }

                // Validate the request
                var validationResult = await _appointmentManagementService.ValidateAppointmentUpdateAsync(id, request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = string.Join(", ", validationResult.Errors)
                    });
                }

                // Update the appointment
                var result = await _appointmentManagementService.UpdateAppointmentAsync(id, request, adminId);
                
                if (!result.Success)
                {
                    return result.Error == "Appointment not found" 
                        ? NotFound(result) 
                        : StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppointmentOperationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Delete an appointment (Admin only)
        /// </summary>
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AppointmentOperationResponse>> AdminDeleteAppointment(Guid id)
        {
            try
            {
                // Get current user ID for audit logging
                var currentUserId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || !Guid.TryParse(currentUserId, out var adminId))
                {
                    return Unauthorized(new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = "Unauthorized access"
                    });
                }

                // Delete the appointment
                var result = await _appointmentManagementService.DeleteAppointmentAsync(id, adminId);
                
                if (!result.Success)
                {
                    return result.Error == "Appointment not found" 
                        ? NotFound(result) 
                        : StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppointmentOperationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                });
            }
        }
        
        /// <summary>
        /// Get appointments with search and pagination (Admin only) - Backward compatible endpoint
        /// </summary>
        [HttpGet("admin/search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PagedAppointmentsResponse>> GetAppointments(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] Guid? doctorId = null,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var request = new AppointmentSearchRequest
                {
                    Search = search,
                    Status = status,
                    Page = page,
                    Limit = Math.Min(limit, 100), // Cap at 100 items per page
                    StartDate = startDate,
                    EndDate = endDate,
                    DoctorId = doctorId,
                    ServiceId = serviceId
                };

                var result = await _appointmentManagementService.GetAppointmentsWithPaginationAsync(request);
                
                if (!result.Success)
                {
                    return StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PagedAppointmentsResponse
                {
                    Success = false,
                    Error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Get appointments with search and pagination (Admin only) - New admin endpoint
        /// </summary>
        [HttpGet("admin/list")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PagedAppointmentsResponse>> AdminGetAppointments(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] Guid? doctorId = null,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var request = new AppointmentSearchRequest
                {
                    Search = search,
                    Status = status,
                    Page = page,
                    Limit = Math.Min(limit, 100), // Cap at 100 items per page
                    StartDate = startDate,
                    EndDate = endDate,
                    DoctorId = doctorId,
                    ServiceId = serviceId
                };

                var result = await _appointmentManagementService.GetAppointmentsWithPaginationAsync(request);
                
                if (!result.Success)
                {
                    return StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PagedAppointmentsResponse
                {
                    Success = false,
                    Error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Get appointment by ID (Admin only)
        /// </summary>
        [HttpGet("admin/appointment/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AppointmentDetailDto>> AdminGetAppointmentById(Guid id)
        {
            try
            {
                var appointment = await _appointmentManagementService.GetAppointmentByIdAsync(id);
                
                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found" });
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message,
                    appointmentId = id
                });
            }
        }

        /// <summary>
        /// Update appointment (Admin only) - BACKWARD COMPATIBLE ENDPOINT
        /// Uses specific route to avoid conflicts with other PUT routes
        /// </summary>
        [HttpPut("update/{id}")]
        [JwtAuthorize]
        public async Task<ActionResult> UpdateAppointmentBackwardCompatible(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                Console.WriteLine($"PUT /api/Appointments/update/{id} - Starting method execution");
                
                // Step 1: Validate JWT token
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"User ID Claim: {userIdClaim}");
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"Invalid user token: {userIdClaim}");
                    return Unauthorized(new { error = "Invalid user token", userIdClaim = userIdClaim });
                }

                // Step 2: Validate request
                if (request == null)
                {
                    Console.WriteLine("Request body is null");
                    return BadRequest(new { error = "Request body is null" });
                }

                Console.WriteLine($"Request received: Status={request.Status}");

                // Step 3: Check if appointment exists
                Console.WriteLine($"Looking for appointment with ID: {id}");
                var existingAppointment = await _context.Appointments.FindAsync(id);
                if (existingAppointment == null)
                {
                    Console.WriteLine($"Appointment not found: {id}");
                    return NotFound(new { error = "Appointment not found", appointmentId = id });
                }

                Console.WriteLine($"Found appointment: {existingAppointment.Id}, Status: {existingAppointment.Status}");

                // Step 4: Test database connection
                var appointmentCount = await _context.Appointments.CountAsync();
                Console.WriteLine($"Database connection test successful. Total appointments: {appointmentCount}");
                
                // Step 5: Update only Status field for minimal testing
                var originalStatus = existingAppointment.Status;
                existingAppointment.Status = request.Status ?? existingAppointment.Status;
                existingAppointment.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"Updating status from '{originalStatus}' to '{existingAppointment.Status}'");

                // Step 6: Save changes
                await _context.SaveChangesAsync();
                Console.WriteLine("Changes saved successfully");

                return Ok(new { 
                    success = true, 
                    message = "Appointment updated successfully", 
                    id = id,
                    appointmentCount = appointmentCount,
                    originalStatus = originalStatus,
                    newStatus = existingAppointment.Status,
                    requestStatus = request.Status
                });
            }
            catch (Exception ex)
            {
                // Comprehensive exception logging
                Console.WriteLine($"PUT /api/Appointments/update/{id} Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }

                var errorDetails = new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message,
                    innerStackTrace = ex.InnerException?.StackTrace,
                    appointmentId = id,
                    requestBody = request?.Status ?? "null",
                    userId = User.FindFirst("sub")?.Value ?? "not found",
                    timestamp = DateTime.UtcNow,
                    exceptionType = ex.GetType().Name
                };

                return StatusCode(500, errorDetails);
            }
        }

        /// <summary>
        /// Update appointment - FRONTEND COMPATIBLE ENDPOINT
        /// This is the route the frontend expects: PUT /api/Appointments/{id}
        /// </summary>
        [HttpPut("{id}")]
        [JwtAuthorize]
        public async Task<ActionResult> UpdateAppointmentFrontendCompatible(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                Console.WriteLine($"PUT /api/Appointments/{id} - Starting method execution (Frontend Compatible)");
                
                // Step 1: Validate JWT token
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"User ID Claim: {userIdClaim}");
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"Invalid user token: {userIdClaim}");
                    return Unauthorized(new { error = "Invalid user token", userIdClaim = userIdClaim });
                }

                // Step 2: Validate request
                if (request == null)
                {
                    Console.WriteLine("Request body is null");
                    return BadRequest(new { error = "Request body is null" });
                }

                Console.WriteLine($"Request received: AppointmentDate={request.AppointmentDate}, StartTime={request.StartTime}, EndTime={request.EndTime}, Notes={request.Notes}, Status={request.Status}");

                // Step 3: Check if appointment exists
                Console.WriteLine($"Looking for appointment with ID: {id}");
                var existingAppointment = await _context.Appointments.FindAsync(id);
                if (existingAppointment == null)
                {
                    Console.WriteLine($"Appointment not found: {id}");
                    return NotFound(new { error = "Appointment not found", appointmentId = id });
                }

                Console.WriteLine($"Found appointment: {existingAppointment.Id}, Status: {existingAppointment.Status}");

                // Step 4: Test database connection
                var appointmentCount = await _context.Appointments.CountAsync();
                Console.WriteLine($"Database connection test successful. Total appointments: {appointmentCount}");
                
                // Step 5: Update all fields from request
                var originalValues = new
                {
                    AppointmentDate = existingAppointment.AppointmentDate,
                    StartTime = existingAppointment.StartTime,
                    EndTime = existingAppointment.EndTime,
                    Notes = existingAppointment.Notes,
                    Status = existingAppointment.Status
                };

                // Update fields only if they are provided in the request
                if (request.AppointmentDate.HasValue)
                {
                    existingAppointment.AppointmentDate = request.AppointmentDate.Value;
                    Console.WriteLine($"Updated AppointmentDate from '{originalValues.AppointmentDate}' to '{existingAppointment.AppointmentDate}'");
                }

                if (request.StartTime.HasValue)
                {
                    existingAppointment.StartTime = request.StartTime.Value;
                    Console.WriteLine($"Updated StartTime from '{originalValues.StartTime}' to '{existingAppointment.StartTime}'");
                }

                if (request.EndTime.HasValue)
                {
                    existingAppointment.EndTime = request.EndTime.Value;
                    Console.WriteLine($"Updated EndTime from '{originalValues.EndTime}' to '{existingAppointment.EndTime}'");
                }

                if (request.Notes != null)
                {
                    existingAppointment.Notes = request.Notes;
                    Console.WriteLine($"Updated Notes from '{originalValues.Notes}' to '{existingAppointment.Notes}'");
                }

                if (request.Status != null)
                {
                    existingAppointment.Status = request.Status;
                    Console.WriteLine($"Updated Status from '{originalValues.Status}' to '{existingAppointment.Status}'");
                }

                existingAppointment.UpdatedAt = DateTime.UtcNow;

                // Step 6: Save changes
                await _context.SaveChangesAsync();
                Console.WriteLine("Changes saved successfully");

                return Ok(new { 
                    success = true, 
                    message = "Appointment updated successfully", 
                    id = id,
                    appointmentCount = appointmentCount,
                    originalValues = originalValues,
                    updatedValues = new
                    {
                        AppointmentDate = existingAppointment.AppointmentDate,
                        StartTime = existingAppointment.StartTime,
                        EndTime = existingAppointment.EndTime,
                        Notes = existingAppointment.Notes,
                        Status = existingAppointment.Status
                    }
                });
            }
            catch (Exception ex)
            {
                // Comprehensive exception logging
                Console.WriteLine($"PUT /api/Appointments/{id} Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }

                var errorDetails = new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message,
                    innerStackTrace = ex.InnerException?.StackTrace,
                    appointmentId = id,
                    requestBody = request?.Status ?? "null",
                    userId = User.FindFirst("sub")?.Value ?? "not found",
                    timestamp = DateTime.UtcNow,
                    exceptionType = ex.GetType().Name
                };

                return StatusCode(500, errorDetails);
            }
        }

        #region Appointment Status Management Endpoints

        /// <summary>
        /// Update appointment status (approve/reject/cancel)
        /// </summary>
        [HttpPut("{id}/status")]
        [AllowAnonymous] // Temporarily allow anonymous for testing
        public async Task<ActionResult<AppointmentStatusResponse>> UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            try
            {
                Console.WriteLine($"PUT /api/Appointments/{id}/status - Starting status update");
                
                // Get user information (handle anonymous access)
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "admin"; // Default to admin for anonymous testing
                
                Console.WriteLine($"User ID: {userIdClaim}, Role: {userRole}");
                
                // For anonymous testing, use a default admin user ID
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = "550e8400-e29b-41d4-a716-446655441001"; // Default admin user ID
                    userRole = "admin";
                    Console.WriteLine($"Using default admin user for anonymous testing: {userIdClaim}");
                }

                // Validate request
                if (request == null)
                {
                    Console.WriteLine("Request body is null");
                    return BadRequest(new { error = "Request body is null" });
                }

                Console.WriteLine($"Status update request: {request.Status}, Reason: {request.Reason}");

                // Update appointment status
                var result = await _appointmentStatusService.UpdateAppointmentStatusAsync(id, request, userIdClaim, userRole);
                
                Console.WriteLine($"Status updated successfully: {result.Status}");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Appointment not found: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {ex.Message}");
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Approve an appointment
        /// </summary>
        [HttpPut("{id}/approve")]
        [AllowAnonymous] // Temporarily allow anonymous for testing
        public async Task<ActionResult<AppointmentActionResponse>> ApproveAppointment(Guid id, [FromBody] ApproveAppointmentRequest request)
        {
            try
            {
                Console.WriteLine($"PUT /api/Appointments/{id}/approve - Starting approval");
                
                // Get user information (handle anonymous access)
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                // For anonymous testing, use a default admin user ID
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = "550e8400-e29b-41d4-a716-446655441001"; // Default admin user ID
                    Console.WriteLine($"Using default admin user for anonymous testing: {userIdClaim}");
                }

                // Approve appointment
                var result = await _appointmentStatusService.ApproveAppointmentAsync(id, request ?? new ApproveAppointmentRequest(), userIdClaim);
                
                Console.WriteLine($"Appointment approved successfully: {result.Id}");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Appointment not found: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {ex.Message}");
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving appointment: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Reject an appointment
        /// </summary>
        [HttpPut("{id}/reject")]
        [AllowAnonymous] // Temporarily allow anonymous for testing
        public async Task<ActionResult<AppointmentActionResponse>> RejectAppointment(Guid id, [FromBody] RejectAppointmentRequest request)
        {
            try
            {
                Console.WriteLine($"PUT /api/Appointments/{id}/reject - Starting rejection");
                
                // Get user information (handle anonymous access)
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                // For anonymous testing, use a default admin user ID
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = "550e8400-e29b-41d4-a716-446655441001"; // Default admin user ID
                    Console.WriteLine($"Using default admin user for anonymous testing: {userIdClaim}");
                }

                // Validate request
                if (request == null || string.IsNullOrEmpty(request.Reason))
                {
                    Console.WriteLine("Rejection reason is required");
                    return BadRequest(new { error = "Rejection reason is required" });
                }

                Console.WriteLine($"Rejection reason: {request.Reason}");

                // Reject appointment
                var result = await _appointmentStatusService.RejectAppointmentAsync(id, request, userIdClaim);
                
                Console.WriteLine($"Appointment rejected successfully: {result.Id}");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Appointment not found: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {ex.Message}");
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting appointment: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get appointment audit log
        /// </summary>
        [HttpGet("{id}/audit-log")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<List<AppointmentAuditLogEntry>>> GetAppointmentAuditLog(Guid id)
        {
            try
            {
                Console.WriteLine($"GET /api/Appointments/{id}/audit-log - Starting audit log retrieval");
                
                // Get user information
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "patient";
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"Invalid user token: {userIdClaim}");
                    return Unauthorized(new { error = "Invalid user token" });
                }

                // Check if user can view this appointment's audit log
                if (!await _appointmentStatusService.CanUserModifyAppointmentAsync(id, userIdClaim, userRole))
                {
                    Console.WriteLine($"Permission denied for user {userIdClaim} to view appointment {id}");
                    return Forbid();
                }

                // Get audit log
                var auditLog = await _appointmentStatusService.GetAppointmentAuditLogAsync(id);
                
                Console.WriteLine($"Retrieved {auditLog.Count} audit log entries");
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving audit log: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        #endregion
    }
}