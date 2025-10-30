using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    public class EnhancedBookingService
    {
        private readonly HopewellDbContext _context;
        private readonly ILogger<EnhancedBookingService> _logger;
        private readonly IDoctorAvailabilityService _availabilityService;

        // Expose availability service for controller access
        public IDoctorAvailabilityService AvailabilityService => _availabilityService;

        public EnhancedBookingService(
            HopewellDbContext context, 
            ILogger<EnhancedBookingService> logger,
            IDoctorAvailabilityService availabilityService)
        {
            _context = context;
            _logger = logger;
            _availabilityService = availabilityService;
        }

        // Enhanced Get Doctors On Duty with shift schedule validation
        public async Task<DoctorOnDutyWithAvailabilityResponse> GetDoctorsOnDutyWithAvailabilityAsync(
            DateTime date, 
            Guid? serviceId = null, 
            bool includeFullyBooked = false)
        {
            try
            {
                // Get all active doctors
                var allDoctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync();

                var doctors = new List<DoctorOnDutyWithAvailabilityDto>();
                var totalAvailableSlots = 0;
                var totalBookedSlots = 0;
                var fullyBookedCount = 0;
                var onDutyCount = 0;
                var offDutyCount = 0;

                foreach (var staff in allDoctors)
                {
                    // Check doctor availability with shift schedule validation
                    var availabilityResult = await _availabilityService.CheckDoctorAvailabilityAsync(staff.Id, date);
                    
                    // Count doctors by status
                    if (availabilityResult.IsOnDuty)
                    {
                        onDutyCount++;
                        totalAvailableSlots += availabilityResult.AvailableSlots;
                        totalBookedSlots += (availabilityResult.TotalSlots - availabilityResult.AvailableSlots);
                        
                        if (availabilityResult.IsFullyBooked)
                        {
                            fullyBookedCount++;
                        }

                        // Skip fully booked doctors if not including them
                        if (!includeFullyBooked && availabilityResult.IsFullyBooked)
                        {
                            continue;
                        }

                        // Get shift schedule info
                        var shiftInfo = await _availabilityService.GetDoctorShiftInfoAsync(staff.Id, date);
                        var shiftStart = shiftInfo?.StartTime != null ? TimeSpan.Parse(shiftInfo.StartTime) : new TimeSpan(9, 0, 0);
                        var shiftEnd = shiftInfo?.EndTime != null ? TimeSpan.Parse(shiftInfo.EndTime) : new TimeSpan(17, 0, 0);

                        doctors.Add(new DoctorOnDutyWithAvailabilityDto
                        {
                            Id = staff.Id,
                            FirstName = staff.User.FirstName,
                            LastName = staff.User.LastName,
                            Email = staff.User.Email ?? "",
                            Specialty = "General Practice",
                            Rating = 4.5,
                            ShiftStart = shiftStart,
                            ShiftEnd = shiftEnd,
                            IsAvailable = availabilityResult.IsAvailable,
                            IsFullyBooked = availabilityResult.IsFullyBooked,
                            AvailableSlots = availabilityResult.AvailableSlots,
                            TotalSlots = availabilityResult.TotalSlots,
                            NextAvailableDate = availabilityResult.NextAvailableDate,
                            Services = GetDoctorServices(staff.Id)
                        });
                    }
                    else
                    {
                        offDutyCount++;
                    }
                }

                return new DoctorOnDutyWithAvailabilityResponse
                {
                    Doctors = doctors,
                    RequestedDate = date,
                    TotalAvailableDoctors = doctors.Count(d => d.IsAvailable),
                    FullyBookedDoctors = fullyBookedCount,
                    TotalAvailableSlots = totalAvailableSlots,
                    TotalBookedSlots = totalBookedSlots
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors on duty with availability for date: {Date}", date);
                return new DoctorOnDutyWithAvailabilityResponse { RequestedDate = date };
            }
        }

        // Enhanced Get Available Slots with doctor availability information
        public async Task<AvailableSlotsWithAvailabilityResponse> GetAvailableSlotsWithAvailabilityAsync(
            Guid doctorId, 
            DateTime date, 
            Guid? serviceId = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                var doctorSchedule = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(ds => ds.DoctorId == doctorId && ds.DayOfWeek == dayOfWeek && ds.IsActive);

                var shiftStart = doctorSchedule?.StartTime ?? new TimeSpan(9, 0, 0);
                var shiftEnd = doctorSchedule?.EndTime ?? new TimeSpan(17, 0, 0);

                var serviceDuration = 30;
                if (serviceId.HasValue)
                {
                    var service = await _context.Services.FindAsync(serviceId.Value);
                    if (service != null)
                    {
                        serviceDuration = service.DurationMinutes;
                    }
                }

                // Get existing appointments for this doctor on this date
                var existingAppointments = await _context.Appointments
                    .Where(a => (a.StaffId == doctorId || a.DoctorId == doctorId) &&
                               a.AppointmentDate == date.Date &&
                               (a.Status == "pending" || a.Status == "confirmed" || 
                                a.Status == "approved" || a.Status == "scheduled" || 
                                a.Status == "walkin"))
                    .ToListAsync();

                var slots = new List<TimeSlotDto>();
                var currentTime = shiftStart;

                // Generate time slots
                while (currentTime.Add(TimeSpan.FromMinutes(serviceDuration)) <= shiftEnd)
                {
                    var endTime = currentTime.Add(TimeSpan.FromMinutes(serviceDuration));
                    var isAvailable = true;

                    // Check if slot conflicts with existing appointments
                    foreach (var appointment in existingAppointments)
                    {
                        var appointmentStart = appointment.StartTime.ToTimeSpan();
                        var appointmentEnd = appointment.EndTime.ToTimeSpan();
                        
                        if (currentTime < appointmentEnd && appointmentStart < endTime)
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    slots.Add(new TimeSlotDto
                    {
                        Id = Guid.NewGuid(),
                        StartTime = currentTime,
                        EndTime = endTime,
                        Duration = serviceDuration,
                        IsAvailable = isAvailable,
                        DoctorId = doctorId
                    });

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(serviceDuration));
                }

                // Get doctor availability information
                var availability = await _availabilityService.GetDoctorAvailabilityAsync(doctorId, date);
                var nextAvailableDate = availability?.IsFullyBooked == true 
                    ? await GetNextAvailableDateAsync(doctorId, date)
                    : null;

                return new AvailableSlotsWithAvailabilityResponse
                {
                    DoctorId = doctorId,
                    Date = date,
                    AvailableSlots = slots.Where(s => s.IsAvailable).ToList(),
                    DoctorAvailability = new DoctorAvailabilityInfo
                    {
                        IsFullyBooked = availability?.IsFullyBooked ?? false,
                        AvailableSlots = availability?.AvailableSlots ?? 0,
                        TotalSlots = availability?.TotalSlots ?? 0,
                        NextAvailableDate = nextAvailableDate
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available slots with availability for doctor: {DoctorId} on date: {Date}", doctorId, date);
                return new AvailableSlotsWithAvailabilityResponse
                {
                    DoctorId = doctorId,
                    Date = date,
                    AvailableSlots = new List<TimeSlotDto>()
                };
            }
        }

        // Get availability summary for a specific date
        public async Task<AvailabilitySummaryResponse> GetAvailabilitySummaryAsync(DateTime date)
        {
            try
            {
                var summary = await _availabilityService.GetAvailabilitySummaryAsync(date);
                var doctors = await _availabilityService.GetDoctorsWithAvailabilityAsync(date);

                return new AvailabilitySummaryResponse
                {
                    Date = date,
                    Summary = summary,
                    Doctors = doctors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability summary for date: {Date}", date);
                return new AvailabilitySummaryResponse { Date = date };
            }
        }

        // Get next available dates for a doctor
        public async Task<NextAvailableDatesResponse> GetNextAvailableDatesAsync(
            Guid doctorId, 
            DateTime? startDate = null, 
            int maxDays = 7)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(1);
                var nextAvailableDates = await _availabilityService.GetNextAvailableDatesAsync(doctorId, maxDays);
                
                // Get doctor name
                var doctor = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == doctorId);

                var doctorName = doctor?.User != null 
                    ? $"{doctor.User.FirstName} {doctor.User.LastName}"
                    : "Unknown Doctor";

                return new NextAvailableDatesResponse
                {
                    DoctorId = doctorId,
                    DoctorName = doctorName,
                    NextAvailableDates = nextAvailableDates,
                    SearchPeriod = new SearchPeriod
                    {
                        StartDate = start,
                        EndDate = start.AddDays(maxDays),
                        DaysSearched = maxDays
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available dates for doctor: {DoctorId}", doctorId);
                return new NextAvailableDatesResponse { DoctorId = doctorId };
            }
        }

        // Get only available doctors (not fully booked)
        public async Task<List<DoctorOnDutyWithAvailabilityDto>> GetAvailableDoctorsAsync(DateTime date, Guid? serviceId = null)
        {
            var response = await GetDoctorsOnDutyWithAvailabilityAsync(date, serviceId, includeFullyBooked: false);
            return response.Doctors.Where(d => !d.IsFullyBooked).ToList();
        }

        // Get only fully booked doctors
        public async Task<List<DoctorOnDutyWithAvailabilityDto>> GetFullyBookedDoctorsAsync(DateTime date, Guid? serviceId = null)
        {
            var response = await GetDoctorsOnDutyWithAvailabilityAsync(date, serviceId, includeFullyBooked: true);
            return response.Doctors.Where(d => d.IsFullyBooked).ToList();
        }

        // Enhanced appointment creation with availability validation
        public async Task<AppointmentBookingResponse> CreateAppointmentWithAvailabilityAsync(CreateBookingAppointmentRequest request)
        {
            // Check doctor availability
            var availability = await _availabilityService.GetDoctorAvailabilityAsync(request.DoctorId, request.Date);
            if (availability?.IsFullyBooked == true)
            {
                throw new InvalidOperationException("DOCTOR_FULLY_BOOKED");
            }

            // Validate doctor is on duty
            var dayOfWeek = request.Date.DayOfWeek.ToString();
            var doctorSchedule = await _context.ShiftSchedules
                .FirstOrDefaultAsync(ds => ds.DoctorId == request.DoctorId && 
                                         ds.DayOfWeek == dayOfWeek &&
                                         ds.IsActive);

            if (doctorSchedule == null)
            {
                throw new InvalidOperationException("DOCTOR_NOT_ON_DUTY");
            }

            // Validate time slot is within doctor's shift
            if (request.StartTime < doctorSchedule.StartTime || request.EndTime > doctorSchedule.EndTime)
            {
                throw new InvalidOperationException("INVALID_APPOINTMENT_TIME");
            }

            // Check for conflicts
            var conflictingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => (a.StaffId == request.DoctorId || a.DoctorId == request.DoctorId) &&
                                         a.AppointmentDate == request.Date.Date &&
                                         a.StartTime < TimeOnly.FromTimeSpan(request.EndTime) &&
                                         a.EndTime > TimeOnly.FromTimeSpan(request.StartTime));

            if (conflictingAppointment != null)
            {
                throw new InvalidOperationException("APPOINTMENT_CONFLICT");
            }

            // Create appointment
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                StaffId = request.DoctorId,
                ServiceId = request.ServiceId ?? Guid.Empty,
                AppointmentDate = request.Date.Date,
                StartTime = TimeOnly.FromTimeSpan(request.StartTime),
                EndTime = TimeOnly.FromTimeSpan(request.EndTime),
                Notes = request.Notes,
                Status = "pending",
                ServicePrice = await GetServicePrice(request.ServiceId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Update doctor availability after creating appointment
            await _availabilityService.UpdateDoctorAvailabilityAsync(request.DoctorId, request.Date);

            return new AppointmentBookingResponse
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.StaffId ?? appointment.DoctorId ?? Guid.Empty,
                Date = appointment.AppointmentDate,
                StartTime = appointment.StartTime.ToTimeSpan(),
                EndTime = appointment.EndTime.ToTimeSpan(),
                Notes = appointment.Notes,
                Status = appointment.Status.ToString(),
                ConfirmationNumber = GenerateConfirmationNumber(),
                CreatedAt = appointment.CreatedAt
            };
        }

        // Helper methods
        public async Task<DateTime?> GetNextAvailableDateAsync(Guid doctorId, DateTime fromDate)
        {
            var nextAvailableDates = await _availabilityService.GetNextAvailableDatesAsync(doctorId, 14);
            return nextAvailableDates.FirstOrDefault();
        }

        private static List<ServiceDto> GetDoctorServices(Guid doctorId)
        {
            return new List<ServiceDto> 
            { 
                new ServiceDto
                {
                    Name = "General Consultation",
                    Description = "General medical consultation",
                    DurationMinutes = 30,
                    Price = 150.00m
                },
                new ServiceDto
                {
                    Name = "Follow-up",
                    Description = "Follow-up appointment",
                    DurationMinutes = 20,
                    Price = 100.00m
                },
                new ServiceDto
                {
                    Name = "Check-up",
                    Description = "Regular health check-up",
                    DurationMinutes = 45,
                    Price = 200.00m
                }
            };
        }

        private async Task<decimal> GetServicePrice(Guid? serviceId)
        {
            if (serviceId.HasValue)
            {
                var service = await _context.Services.FindAsync(serviceId.Value);
                return service?.Price ?? 0;
            }
            return 0;
        }

        private string GenerateConfirmationNumber()
        {
            return $"APT{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
