using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.Services;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Attributes;
using HopewellClinicApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        private readonly HopewellDbContext _context;

        public BookingController(BookingService bookingService, ILogger<BookingController> logger, HopewellDbContext context)
        {
            _bookingService = bookingService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Step 1: Validate selected appointment date
        /// </summary>
        [HttpPost("validate-date")]
        [AllowAnonymous]
        public async Task<ActionResult<DateValidationResponse>> ValidateDate([FromBody] DateValidationRequest request)
        {
            try
            {
                var response = await _bookingService.ValidateDateAsync(request.Date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating date: {Date}", request.Date);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "DATE_VALIDATION_ERROR",
                    Message = "An error occurred while validating the date."
                });
            }
        }

        /// <summary>
        /// Step 2: Get doctors on duty for a specific date
        /// </summary>
        [HttpGet("doctors-on-duty")]
        [AllowAnonymous]
        public async Task<ActionResult<DoctorOnDutyResponse>> GetDoctorsOnDuty([FromQuery] DateTime date, [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var response = await _bookingService.GetDoctorsOnDutyAsync(date, serviceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors on duty for date: {Date}", date);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "DOCTORS_ON_DUTY_ERROR",
                    Message = "An error occurred while retrieving doctors on duty."
                });
            }
        }

        /// <summary>
        /// Step 3: Get available time slots for a specific doctor on a specific date
        /// </summary>
        [HttpGet("available-slots-by-doctor")]
        [AllowAnonymous]
        public async Task<ActionResult<AvailableSlotsByDoctorResponse>> GetAvailableSlotsByDoctor(
            [FromQuery] Guid doctorId, 
            [FromQuery] DateTime date, 
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var response = await _bookingService.GetAvailableSlotsByDoctorAsync(doctorId, date, serviceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available slots for doctor: {DoctorId} on date: {Date}", doctorId, date);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "AVAILABLE_SLOTS_ERROR",
                    Message = "An error occurred while retrieving available time slots."
                });
            }
        }

        /// <summary>
        /// Step 4: Get staff on duty for a specific date
        /// </summary>
        [HttpGet("staff-on-duty")]
        [AllowAnonymous]
        public async Task<ActionResult<StaffOnDutyResponse>> GetStaffOnDuty([FromQuery] DateTime date, [FromQuery] string? role = null)
        {
            try
            {
                var response = await _bookingService.GetStaffOnDutyAsync(date, role);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff on duty for date: {Date}", date);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "STAFF_ON_DUTY_ERROR",
                    Message = "An error occurred while retrieving staff on duty."
                });
            }
        }

        /// <summary>
        /// Step 5: Create appointment with enhanced validation
        /// </summary>
        [HttpPost("create-appointment")]
        [JwtAuthorize]
        public async Task<ActionResult<AppointmentBookingResponse>> CreateAppointment([FromBody] CreateBookingAppointmentRequest request)
        {
            try
            {
                var response = await _bookingService.CreateAppointmentAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex) when (ex.Message == "DOCTOR_NOT_ON_DUTY")
            {
                return BadRequest(new BookingErrorResponse
                {
                    Error = "DOCTOR_NOT_ON_DUTY",
                    Message = "The selected doctor is not on duty on the chosen date.",
                    Details = new { DoctorId = request.DoctorId, Date = request.Date }
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "INVALID_APPOINTMENT_TIME")
            {
                return BadRequest(new BookingErrorResponse
                {
                    Error = "INVALID_APPOINTMENT_TIME",
                    Message = "The selected time is outside the doctor's working hours.",
                    Details = new { DoctorId = request.DoctorId, StartTime = request.StartTime, EndTime = request.EndTime }
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "APPOINTMENT_CONFLICT")
            {
                return BadRequest(new BookingErrorResponse
                {
                    Error = "APPOINTMENT_CONFLICT",
                    Message = "The selected time slot is no longer available.",
                    Details = new { DoctorId = request.DoctorId, Date = request.Date, StartTime = request.StartTime }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for patient: {PatientId}", request.PatientId);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "APPOINTMENT_CREATION_ERROR",
                    Message = "An error occurred while creating the appointment."
                });
            }
        }

        /// <summary>
        /// Get booking summary for a specific date
        /// </summary>
        [HttpGet("booking-summary")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetBookingSummary([FromQuery] DateTime date)
        {
            try
            {
                var doctorsResponse = await _bookingService.GetDoctorsOnDutyAsync(date);
                var staffResponse = await _bookingService.GetStaffOnDutyAsync(date);

                var summary = new
                {
                    Date = date,
                    AvailableDoctors = doctorsResponse.Doctors.Count,
                    TotalStaff = staffResponse.Staff.Count,
                    Doctors = doctorsResponse.Doctors.Select(d => new
                    {
                        d.Id,
                        d.FirstName,
                        d.LastName,
                        d.Specialty,
                        d.ShiftStart,
                        d.ShiftEnd
                    }),
                    Staff = staffResponse.Staff.Select(s => new
                    {
                        s.Id,
                        s.FirstName,
                        s.LastName,
                        s.Role,
                        s.Specialty
                    })
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking summary for date: {Date}", date);
                return StatusCode(500, new BookingErrorResponse
                {
                    Error = "BOOKING_SUMMARY_ERROR",
                    Message = "An error occurred while retrieving booking summary."
                });
            }
        }

        /// <summary>
        /// Simple fallback: Get all available doctors
        /// </summary>
        [HttpGet("available-doctors")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAvailableDoctors()
        {
            try
            {
                var doctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .Select(s => new
                    {
                        id = s.Id,
                        firstName = s.User.FirstName,
                        lastName = s.User.LastName,
                        role = "doctor",
                        specialty = "General",
                        isAvailable = true
                    })
                    .ToListAsync();

                return Ok(new { doctors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available doctors");
                return StatusCode(500, new { error = "DOCTORS_ERROR", message = "An error occurred while retrieving doctors." });
            }
        }

        /// <summary>
        /// Debug endpoint to test database connection and data
        /// </summary>
        [HttpGet("debug")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Debug()
        {
            try
            {
                var debugInfo = new
                {
                    DatabaseConnection = "Connected",
                    StaffCount = await _context.Staff.CountAsync(),
                    ActiveStaffCount = await _context.Staff.Where(s => s.IsActive).CountAsync(),
                    DoctorSchedulesCount = await _context.ShiftSchedules.CountAsync(),
                    AppointmentsCount = await _context.Appointments.CountAsync(),
                    ServicesCount = await _context.Services.CountAsync(),
                    StaffWithUsers = await _context.Staff.Include(s => s.User).Where(s => s.User != null).CountAsync(),
                    SampleStaff = await _context.Staff
                        .Include(s => s.User)
                        .Where(s => s.IsActive)
                        .Take(3)
                        .Select(s => new
                        {
                            id = s.Id,
                            firstName = s.User != null ? s.User.FirstName : "No User",
                            lastName = s.User != null ? s.User.LastName : "No User",
                            isActive = s.IsActive
                        })
                        .ToListAsync()
                };

                return Ok(debugInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug endpoint");
                return StatusCode(500, new { error = "DEBUG_ERROR", message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Mock endpoint for testing - returns sample data
        /// </summary>
        [HttpGet("mock-doctors")]
        [AllowAnonymous]
        public ActionResult<object> GetMockDoctors()
        {
            var mockDoctors = new[]
            {
                new
                {
                    id = "550e8400-e29b-41d4-a716-446655441000",
                    firstName = "John",
                    lastName = "Smith",
                    specialty = "General Practice",
                    rating = 4.5,
                    shiftStart = "09:00:00",
                    shiftEnd = "17:00:00",
                    isAvailable = true,
                    services = new[] { "consultation", "follow-up", "check-up" }
                },
                new
                {
                    id = "550e8400-e29b-41d4-a716-446655441001",
                    firstName = "Jane",
                    lastName = "Doe",
                    specialty = "General Practice",
                    rating = 4.8,
                    shiftStart = "09:00:00",
                    shiftEnd = "17:00:00",
                    isAvailable = true,
                    services = new[] { "consultation", "follow-up", "check-up" }
                }
            };

            return Ok(new { doctors = mockDoctors });
        }

        /// <summary>
        /// Mock endpoint for testing - returns sample time slots
        /// </summary>
        [HttpGet("mock-slots")]
        [AllowAnonymous]
        public ActionResult<object> GetMockSlots([FromQuery] string doctorId = "550e8400-e29b-41d4-a716-446655441000", [FromQuery] string date = "2025-09-19")
        {
            var mockSlots = new[]
            {
                new { id = "slot1", startTime = "09:00:00", endTime = "09:30:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot2", startTime = "09:30:00", endTime = "10:00:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot3", startTime = "10:00:00", endTime = "10:30:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot4", startTime = "10:30:00", endTime = "11:00:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot5", startTime = "11:00:00", endTime = "11:30:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot6", startTime = "11:30:00", endTime = "12:00:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot7", startTime = "14:00:00", endTime = "14:30:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot8", startTime = "14:30:00", endTime = "15:00:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot9", startTime = "15:00:00", endTime = "15:30:00", duration = 30, isAvailable = true, doctorId = doctorId },
                new { id = "slot10", startTime = "15:30:00", endTime = "16:00:00", duration = 30, isAvailable = true, doctorId = doctorId }
            };

            return Ok(new { availableSlots = mockSlots });
        }

        /// <summary>
        /// Debug endpoint to show database info
        /// </summary>
        [HttpGet("debug-database")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDebugDatabase()
        {
            try
            {
                var totalAppointments = await _context.Appointments.CountAsync();
                var totalPatients = await _context.Patients.CountAsync();
                var totalStaff = await _context.Staff.CountAsync();
                var totalServices = await _context.Services.CountAsync();
                
                var sampleAppointments = await _context.Appointments
                    .Take(10)
                    .Select(a => new
                    {
                        id = a.Id,
                        appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                        startTime = a.StartTime.ToString("HH:mm:ss"),
                        endTime = a.EndTime.ToString("HH:mm:ss"),
                        staffId = a.StaffId,
                        doctorId = a.DoctorId,
                        status = a.Status,
                        patientId = a.PatientId
                    })
                    .ToListAsync();

                // Get all appointments for Dr. Brown specifically
                var drBrownAppointments = await _context.Appointments
                    .Where(a => a.StaffId == Guid.Parse("ee8bf9c2-3ef6-4081-9815-4b91b3b07620") || 
                               a.DoctorId == Guid.Parse("ee8bf9c2-3ef6-4081-9815-4b91b3b07620"))
                    .Select(a => new
                    {
                        id = a.Id,
                        appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                        startTime = a.StartTime.ToString("HH:mm:ss"),
                        endTime = a.EndTime.ToString("HH:mm:ss"),
                        staffId = a.StaffId,
                        doctorId = a.DoctorId,
                        status = a.Status,
                        patientId = a.PatientId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    totalAppointments,
                    totalPatients,
                    totalStaff,
                    totalServices,
                    sampleAppointments,
                    drBrownAppointments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting debug database info");
                return StatusCode(500, new { error = "DEBUG_DATABASE_ERROR", message = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint to show existing appointments for a doctor on a specific date
        /// </summary>
        [HttpGet("debug-appointments")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDebugAppointments([FromQuery] string doctorId, [FromQuery] string date)
        {
            try
            {
                if (!Guid.TryParse(doctorId, out var doctorGuid))
                {
                    return BadRequest("Invalid doctor ID format");
                }

                if (!DateTime.TryParse(date, out var appointmentDate))
                {
                    return BadRequest("Invalid date format");
                }

                var appointments = await _context.Appointments
                    .Where(a => (a.StaffId == doctorGuid || a.DoctorId == doctorGuid) && 
                               a.AppointmentDate == appointmentDate.Date &&
                               (a.Status == "pending" || a.Status == "confirmed" || a.Status == "approved" || a.Status == "scheduled"))
                    .Select(a => new
                    {
                        id = a.Id,
                        startTime = a.StartTime.ToString("HH:mm:ss"),
                        endTime = a.EndTime.ToString("HH:mm:ss"),
                        patientId = a.PatientId,
                        status = a.Status,
                        notes = a.Notes,
                        staffId = a.StaffId,
                        doctorId = a.DoctorId
                    })
                    .ToListAsync();

                // Also get all appointments for this doctor regardless of date to see what exists
                var allAppointments = await _context.Appointments
                    .Where(a => a.StaffId == doctorGuid || a.DoctorId == doctorGuid)
                    .Select(a => new
                    {
                        id = a.Id,
                        appointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                        startTime = a.StartTime.ToString("HH:mm:ss"),
                        endTime = a.EndTime.ToString("HH:mm:ss"),
                        patientId = a.PatientId,
                        status = a.Status,
                        staffId = a.StaffId,
                        doctorId = a.DoctorId
                    })
                    .Take(10)
                    .ToListAsync();

                return Ok(new
                {
                    doctorId = doctorId,
                    date = appointmentDate.ToString("yyyy-MM-dd"),
                    appointmentCount = appointments.Count,
                    appointments = appointments,
                    allAppointmentsForDoctor = allAppointments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting debug appointments for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return StatusCode(500, new { error = "DEBUG_APPOINTMENTS_ERROR", message = ex.Message });
            }
        }
    }
}
