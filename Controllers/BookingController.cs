using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.Services;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Attributes;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly EnhancedBookingService _enhancedBookingService;
        private readonly ILogger<BookingController> _logger;
        private readonly HopewellDbContext _context;

        public BookingController(
            BookingService bookingService, 
            EnhancedBookingService enhancedBookingService,
            ILogger<BookingController> logger, 
            HopewellDbContext context)
        {
            _bookingService = bookingService;
            _enhancedBookingService = enhancedBookingService;
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

        // ===== ENHANCED AVAILABILITY ENDPOINTS =====

        /// <summary>
        /// Enhanced: Get doctors on duty with availability information
        /// </summary>
        [HttpGet("doctors-on-duty-enhanced")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<DoctorOnDutyWithAvailabilityResponse>>> GetDoctorsOnDutyEnhanced(
            [FromQuery] DateTime date, 
            [FromQuery] Guid? serviceId = null,
            [FromQuery] bool includeFullyBooked = false)
        {
            try
            {
                var response = await _enhancedBookingService.GetDoctorsOnDutyWithAvailabilityAsync(date, serviceId, includeFullyBooked);
                return Ok(new ApiResponse<DoctorOnDutyWithAvailabilityResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enhanced doctors on duty for date: {Date}", date);
                return StatusCode(500, new ApiResponse<DoctorOnDutyWithAvailabilityResponse>
                {
                    Success = false,
                    Error = "DOCTORS_ON_DUTY_ERROR",
                    Message = "An error occurred while retrieving doctors on duty."
                });
            }
        }

        /// <summary>
        /// Enhanced: Get available time slots with doctor availability information
        /// </summary>
        [HttpGet("available-slots-enhanced")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AvailableSlotsWithAvailabilityResponse>>> GetAvailableSlotsEnhanced(
            [FromQuery] Guid doctorId, 
            [FromQuery] DateTime date, 
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var response = await _enhancedBookingService.GetAvailableSlotsWithAvailabilityAsync(doctorId, date, serviceId);
                return Ok(new ApiResponse<AvailableSlotsWithAvailabilityResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enhanced available slots for doctor: {DoctorId} on date: {Date}", doctorId, date);
                return StatusCode(500, new ApiResponse<AvailableSlotsWithAvailabilityResponse>
                {
                    Success = false,
                    Error = "AVAILABLE_SLOTS_ERROR",
                    Message = "An error occurred while retrieving available time slots."
                });
            }
        }

        /// <summary>
        /// Get availability summary for a specific date
        /// </summary>
        [HttpGet("availability-summary")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AvailabilitySummaryResponse>>> GetAvailabilitySummary(
            [FromQuery] DateTime date)
        {
            try
            {
                var response = await _enhancedBookingService.GetAvailabilitySummaryAsync(date);
                return Ok(new ApiResponse<AvailabilitySummaryResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability summary for date: {Date}", date);
                return StatusCode(500, new ApiResponse<AvailabilitySummaryResponse>
                {
                    Success = false,
                    Error = "AVAILABILITY_SUMMARY_ERROR",
                    Message = "An error occurred while retrieving availability summary."
                });
            }
        }

        /// <summary>
        /// Get next available dates for a specific doctor
        /// </summary>
        [HttpGet("next-available-dates/{doctorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<NextAvailableDatesResponse>>> GetNextAvailableDates(
            Guid doctorId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] int maxDays = 7)
        {
            try
            {
                var response = await _enhancedBookingService.GetNextAvailableDatesAsync(doctorId, startDate, maxDays);
                return Ok(new ApiResponse<NextAvailableDatesResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available dates for doctor: {DoctorId}", doctorId);
                return StatusCode(500, new ApiResponse<NextAvailableDatesResponse>
                {
                    Success = false,
                    Error = "NEXT_AVAILABLE_DATES_ERROR",
                    Message = "An error occurred while retrieving next available dates."
                });
            }
        }

        /// <summary>
        /// Get only available doctors (not fully booked)
        /// </summary>
        [HttpGet("available-doctors-only")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>>> GetAvailableDoctorsOnly(
            [FromQuery] DateTime date,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var response = await _enhancedBookingService.GetAvailableDoctorsAsync(date, serviceId);
                return Ok(new ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available doctors only for date: {Date}", date);
                return StatusCode(500, new ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>
                {
                    Success = false,
                    Error = "AVAILABLE_DOCTORS_ERROR",
                    Message = "An error occurred while retrieving available doctors."
                });
            }
        }

        /// <summary>
        /// Get only fully booked doctors
        /// </summary>
        [HttpGet("fully-booked-doctors")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>>> GetFullyBookedDoctors(
            [FromQuery] DateTime date,
            [FromQuery] Guid? serviceId = null)
        {
            try
            {
                var response = await _enhancedBookingService.GetFullyBookedDoctorsAsync(date, serviceId);
                return Ok(new ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fully booked doctors for date: {Date}", date);
                return StatusCode(500, new ApiResponse<List<DoctorOnDutyWithAvailabilityDto>>
                {
                    Success = false,
                    Error = "FULLY_BOOKED_DOCTORS_ERROR",
                    Message = "An error occurred while retrieving fully booked doctors."
                });
            }
        }

        /// <summary>
        /// Enhanced: Create appointment with availability validation
        /// </summary>
        [HttpPost("create-appointment-enhanced")]
        [JwtAuthorize]
        public async Task<ActionResult<ApiResponse<AppointmentBookingResponse>>> CreateAppointmentEnhanced([FromBody] CreateBookingAppointmentRequest request)
        {
            try
            {
                var response = await _enhancedBookingService.CreateAppointmentWithAvailabilityAsync(request);
                return Ok(new ApiResponse<AppointmentBookingResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Appointment created successfully"
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "DOCTOR_FULLY_BOOKED")
            {
                return BadRequest(new ApiResponse<AppointmentBookingResponse>
                {
                    Success = false,
                    Error = "DOCTOR_FULLY_BOOKED",
                    Message = "The selected doctor is fully booked on the chosen date.",
                    Data = null
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "DOCTOR_NOT_ON_DUTY")
            {
                return BadRequest(new ApiResponse<AppointmentBookingResponse>
                {
                    Success = false,
                    Error = "DOCTOR_NOT_ON_DUTY",
                    Message = "The selected doctor is not on duty on the chosen date.",
                    Data = null
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "INVALID_APPOINTMENT_TIME")
            {
                return BadRequest(new ApiResponse<AppointmentBookingResponse>
                {
                    Success = false,
                    Error = "INVALID_APPOINTMENT_TIME",
                    Message = "The selected time is outside the doctor's working hours.",
                    Data = null
                });
            }
            catch (InvalidOperationException ex) when (ex.Message == "APPOINTMENT_CONFLICT")
            {
                return BadRequest(new ApiResponse<AppointmentBookingResponse>
                {
                    Success = false,
                    Error = "APPOINTMENT_CONFLICT",
                    Message = "The selected time slot is no longer available.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enhanced appointment for patient: {PatientId}", request.PatientId);
                return StatusCode(500, new ApiResponse<AppointmentBookingResponse>
                {
                    Success = false,
                    Error = "APPOINTMENT_CREATION_ERROR",
                    Message = "An error occurred while creating the appointment."
                });
            }
        }

        // ===== SHIFT SCHEDULE INTEGRATION ENDPOINTS =====

        /// <summary>
        /// Check doctor availability with shift schedule validation
        /// </summary>
        [HttpGet("doctor-availability-check")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<DoctorAvailabilityCheckResponse>>> CheckDoctorAvailability(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime date)
        {
            try
            {
                var availabilityResult = await _enhancedBookingService.AvailabilityService.CheckDoctorAvailabilityAsync(doctorId, date);
                
                // Get alternative doctors if not available
                var alternativeDoctors = new List<AlternativeDoctor>();
                if (!availabilityResult.IsAvailable)
                {
                    var allDoctors = await _context.Staff
                        .Include(s => s.User)
                        .Where(s => s.IsActive && s.Id != doctorId)
                        .ToListAsync();

                    foreach (var staff in allDoctors.Take(3)) // Limit to 3 alternatives
                    {
                        var altAvailability = await _enhancedBookingService.AvailabilityService.CheckDoctorAvailabilityAsync(staff.Id, date);
                        if (altAvailability.IsAvailable)
                        {
                            alternativeDoctors.Add(new AlternativeDoctor
                            {
                                Id = staff.Id,
                                Name = $"{staff.User.FirstName} {staff.User.LastName}",
                                Specialty = "General Practice",
                                AvailableSlots = altAvailability.AvailableSlots,
                                Rating = 4.5
                            });
                        }
                    }
                }

                var response = new DoctorAvailabilityCheckResponse
                {
                    DoctorId = doctorId,
                    Date = date,
                    IsOnDuty = availabilityResult.IsOnDuty,
                    IsAvailable = availabilityResult.IsAvailable,
                    Reason = availabilityResult.UnavailabilityReason,
                    ShiftSchedule = availabilityResult.ShiftSchedule,
                    NextAvailableDate = availabilityResult.NextAvailableDate,
                    AlternativeDoctors = alternativeDoctors
                };

                return Ok(new ApiResponse<DoctorAvailabilityCheckResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking doctor availability for doctor: {DoctorId} on date: {Date}", doctorId, date);
                return StatusCode(500, new ApiResponse<DoctorAvailabilityCheckResponse>
                {
                    Success = false,
                    Error = "AVAILABILITY_CHECK_ERROR",
                    Message = "An error occurred while checking doctor availability."
                });
            }
        }

        /// <summary>
        /// Get doctor shift schedule
        /// </summary>
        [HttpGet("doctor/{doctorId}/shifts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<DoctorShiftScheduleResponse>>> GetDoctorShiftSchedule(Guid doctorId)
        {
            try
            {
                // Get doctor info
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new ApiResponse<DoctorShiftScheduleResponse>
                    {
                        Success = false,
                        Error = "DOCTOR_NOT_FOUND",
                        Message = "Doctor not found"
                    });
                }

                // Get shift schedules
                var shifts = await _context.ShiftSchedules
                    .Where(s => s.DoctorId == doctorId)
                    .OrderBy(s => s.DayOfWeek)
                    .ToListAsync();

                var shiftInfos = shifts.Select(s => new DoctorShiftInfo
                {
                    Id = s.Id,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm"),
                    IsActive = s.IsActive,
                    BreakStartTime = null,
                    BreakEndTime = null,
                    EffectiveFrom = null,
                    EffectiveTo = null
                }).ToList();

                // Get next available date
                var nextAvailableDate = await _enhancedBookingService.GetNextAvailableDateAsync(doctorId, DateTime.Today);

                var response = new DoctorShiftScheduleResponse
                {
                    DoctorId = doctorId,
                    DoctorName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                    Shifts = shiftInfos,
                    NextAvailableDate = nextAvailableDate,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(new ApiResponse<DoctorShiftScheduleResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor shift schedule for doctor: {DoctorId}", doctorId);
                return StatusCode(500, new ApiResponse<DoctorShiftScheduleResponse>
                {
                    Success = false,
                    Error = "SHIFT_SCHEDULE_ERROR",
                    Message = "An error occurred while retrieving doctor shift schedule."
                });
            }
        }

        /// <summary>
        /// Update doctor shift schedule
        /// </summary>
        [HttpPut("doctor/{doctorId}/shifts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> UpdateDoctorShiftSchedule(Guid doctorId, [FromBody] UpdateShiftScheduleRequest request)
        {
            try
            {
                // Get doctor info
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "DOCTOR_NOT_FOUND",
                        Message = "Doctor not found"
                    });
                }

                // Get existing shifts
                var existingShifts = await _context.ShiftSchedules
                    .Where(s => s.DoctorId == doctorId)
                    .ToListAsync();

                // Update or create shifts
                foreach (var shiftInfo in request.Shifts)
                {
                    var existingShift = existingShifts.FirstOrDefault(s => s.DayOfWeek == shiftInfo.DayOfWeek);
                    
                    if (existingShift != null)
                    {
                        // Update existing shift
                        existingShift.StartTime = TimeSpan.Parse(shiftInfo.StartTime);
                        existingShift.EndTime = TimeSpan.Parse(shiftInfo.EndTime);
                        existingShift.IsActive = shiftInfo.IsActive;
                        existingShift.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new shift
                        var newShift = new ShiftSchedule
                        {
                            Id = Guid.NewGuid(),
                            DoctorId = doctorId,
                            DayOfWeek = shiftInfo.DayOfWeek,
                            StartTime = TimeSpan.Parse(shiftInfo.StartTime),
                            EndTime = TimeSpan.Parse(shiftInfo.EndTime),
                            IsActive = shiftInfo.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.ShiftSchedules.Add(newShift);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new { message = "Shift schedule updated successfully" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor shift schedule for doctor: {DoctorId}", doctorId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Error = "SHIFT_SCHEDULE_UPDATE_ERROR",
                    Message = "An error occurred while updating doctor shift schedule."
                });
            }
        }

        /// <summary>
        /// Get doctors with shift schedule validation (enhanced version)
        /// </summary>
        [HttpGet("doctors-with-shift-validation")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<DoctorsWithShiftValidationResponse>>> GetDoctorsWithShiftValidation(
            [FromQuery] DateTime date,
            [FromQuery] bool includeOffDuty = false)
        {
            try
            {
                var allDoctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync();

                var doctors = new List<DoctorWithShiftValidation>();
                var onDutyCount = 0;
                var offDutyCount = 0;
                var availableCount = 0;
                var fullyBookedCount = 0;
                var totalAvailableSlots = 0;
                var totalBookedSlots = 0;

                foreach (var staff in allDoctors)
                {
                    var availabilityResult = await _enhancedBookingService.AvailabilityService.CheckDoctorAvailabilityAsync(staff.Id, date);
                    
                    if (availabilityResult.IsOnDuty)
                    {
                        onDutyCount++;
                        if (availabilityResult.IsAvailable) availableCount++;
                        if (availabilityResult.IsFullyBooked) fullyBookedCount++;
                        totalAvailableSlots += availabilityResult.AvailableSlots;
                        totalBookedSlots += (availabilityResult.TotalSlots - availabilityResult.AvailableSlots);
                    }
                    else
                    {
                        offDutyCount++;
                    }

                    // Include off-duty doctors if requested
                    if (!availabilityResult.IsOnDuty && !includeOffDuty)
                    {
                        continue;
                    }

                    // Get shift info
                    var shiftInfo = await _enhancedBookingService.AvailabilityService.GetDoctorShiftInfoAsync(staff.Id, date);
                    var shiftStart = shiftInfo?.StartTime ?? "09:00";
                    var shiftEnd = shiftInfo?.EndTime ?? "17:00";

                    doctors.Add(new DoctorWithShiftValidation
                    {
                        Id = staff.Id,
                        FirstName = staff.User.FirstName,
                        LastName = staff.User.LastName,
                        Email = staff.User.Email ?? "",
                        Specialty = "General Practice",
                        Rating = 4.5,
                        ShiftStart = shiftStart,
                        ShiftEnd = shiftEnd,
                        IsOnDuty = availabilityResult.IsOnDuty,
                        IsAvailable = availabilityResult.IsAvailable,
                        IsFullyBooked = availabilityResult.IsFullyBooked,
                        AvailableSlots = availabilityResult.AvailableSlots,
                        TotalSlots = availabilityResult.TotalSlots,
                        UnavailabilityReason = availabilityResult.UnavailabilityReason,
                        NextAvailableDate = availabilityResult.NextAvailableDate,
                        Services = GetDoctorServices(staff.Id)
                    });
                }

                var summary = new EnhancedDoctorAvailabilitySummary
                {
                    Date = date,
                    TotalDoctors = allDoctors.Count,
                    OnDutyDoctors = onDutyCount,
                    OffDutyDoctors = offDutyCount,
                    AvailableDoctors = availableCount,
                    FullyBookedDoctors = fullyBookedCount,
                    TotalAvailableSlots = totalAvailableSlots,
                    TotalBookedSlots = totalBookedSlots
                };

                var response = new DoctorsWithShiftValidationResponse
                {
                    Doctors = doctors,
                    RequestedDate = date,
                    Summary = summary
                };

                return Ok(new ApiResponse<DoctorsWithShiftValidationResponse>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors with shift validation for date: {Date}", date);
                return StatusCode(500, new ApiResponse<DoctorsWithShiftValidationResponse>
                {
                    Success = false,
                    Error = "DOCTORS_SHIFT_VALIDATION_ERROR",
                    Message = "An error occurred while retrieving doctors with shift validation."
                });
            }
        }

        /// <summary>
        /// TEMPORARY: Update Dr. John Smith's weekend shifts to inactive
        /// </summary>
        [HttpPost("update-dr-john-shifts")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateDrJohnShifts()
        {
            try
            {
                var doctorId = Guid.Parse("42f78af2-c1c5-486c-9de5-0e7e44a8f0da");
                
                // Update Friday shift to inactive
                var fridayShift = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == "Friday");
                
                if (fridayShift != null)
                {
                    fridayShift.IsActive = false;
                    fridayShift.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Updated Friday shift to inactive for Dr. John Smith");
                }
                
                // Update Saturday shift to inactive
                var saturdayShift = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == "Saturday");
                
                if (saturdayShift != null)
                {
                    saturdayShift.IsActive = false;
                    saturdayShift.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Updated Saturday shift to inactive for Dr. John Smith");
                }
                
                // Update Sunday shift to inactive
                var sundayShift = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == "Sunday");
                
                if (sundayShift != null)
                {
                    sundayShift.IsActive = false;
                    sundayShift.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Updated Sunday shift to inactive for Dr. John Smith");
                }
                
                await _context.SaveChangesAsync();
                
                return Ok(new { 
                    success = true, 
                    message = "Dr. John Smith's weekend shifts updated successfully!",
                    updatedShifts = new[] { "Friday", "Saturday", "Sunday" },
                    doctorId = doctorId.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Dr. John Smith's shifts");
                return StatusCode(500, new { success = false, message = "Error updating shifts", error = ex.Message });
            }
        }

        // Helper method to get doctor services
        private static List<ServiceDto> GetDoctorServices(Guid doctorId)
        {
            return new List<ServiceDto> 
            { 
                new ServiceDto
                {
                    Name = "General Consultation",
                    Description = "General medical consultation",
                    DurationMinutes = 30,
                    Price = 150.00m,
                    IsActive = true
                },
                new ServiceDto
                {
                    Name = "Follow-up",
                    Description = "Follow-up appointment",
                    DurationMinutes = 20,
                    Price = 100.00m,
                    IsActive = true
                },
                new ServiceDto
                {
                    Name = "Check-up",
                    Description = "Regular health check-up",
                    DurationMinutes = 45,
                    Price = 200.00m,
                    IsActive = true
                }
            };
        }
    }
}
