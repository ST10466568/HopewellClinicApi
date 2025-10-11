using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    public class BookingService
    {
        private readonly HopewellDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(HopewellDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Step 1: Validate selected date
        public async Task<DateValidationResponse> ValidateDateAsync(DateTime date)
        {
            var response = new DateValidationResponse();

            // Check if date is in the past
            if (date.Date < DateTime.Today)
            {
                response.IsValid = false;
                response.Message = "Cannot book appointments for past dates.";
                return response;
            }

            // Check if date is within booking window (30 days)
            if (date.Date > DateTime.Today.AddDays(30))
            {
                response.IsValid = false;
                response.Message = "Cannot book appointments more than 30 days in advance.";
                return response;
            }

            // Check if date is not a weekend (optional business rule)
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                response.IsValid = false;
                response.Message = "Clinic is closed on weekends.";
                response.SuggestedDates = GetNextWeekdayDates(date, 3);
                return response;
            }

            response.IsValid = true;
            response.Message = "Date is valid for booking.";
            return response;
        }

        // Step 2: Get doctors on duty for a specific date
        public async Task<DoctorOnDutyResponse> GetDoctorsOnDutyAsync(DateTime date, Guid? serviceId = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                
                // First, try to get doctors with specific schedules for this date
                var doctorsWithSchedules = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .Join(_context.ShiftSchedules,
                        s => s.Id,
                        ds => ds.DoctorId,
                        (s, ds) => new { Staff = s, Schedule = ds })
                    .Where(x => x.Schedule.DayOfWeek == dayOfWeek && x.Schedule.IsActive)
                    .Select(x => new DoctorOnDutyDto
                    {
                        Id = x.Staff.Id,
                        FirstName = x.Staff.User.FirstName,
                        LastName = x.Staff.User.LastName,
                        Specialty = "General Practice",
                        Rating = 4.5,
                        ShiftStart = x.Schedule.StartTime,
                        ShiftEnd = x.Schedule.EndTime,
                        IsAvailable = true,
                        Services = GetDoctorServices(x.Staff.Id)
                    })
                    .ToListAsync();

                // If no doctors with specific schedules, fall back to all active doctors
                if (!doctorsWithSchedules.Any())
                {
                    var fallbackDoctors = await _context.Staff
                        .Include(s => s.User)
                        .Where(s => s.IsActive)
                        .Select(s => new DoctorOnDutyDto
                        {
                            Id = s.Id,
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Specialty = "General Practice",
                            Rating = 4.5,
                            ShiftStart = new TimeSpan(9, 0, 0), // Default 9 AM
                            ShiftEnd = new TimeSpan(17, 0, 0), // Default 5 PM
                            IsAvailable = true,
                            Services = GetDoctorServices(s.Id)
                        })
                        .ToListAsync();

                    return new DoctorOnDutyResponse { Doctors = fallbackDoctors };
                }

                return new DoctorOnDutyResponse { Doctors = doctorsWithSchedules };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors on duty for date: {Date}", date);
                
                // Fallback to basic doctor list if there's an error
                try
                {
                    var fallbackDoctors = await _context.Staff
                        .Include(s => s.User)
                        .Where(s => s.IsActive)
                        .Select(s => new DoctorOnDutyDto
                        {
                            Id = s.Id,
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Specialty = "General Practice",
                            Rating = 4.5,
                            ShiftStart = new TimeSpan(9, 0, 0),
                            ShiftEnd = new TimeSpan(17, 0, 0),
                            IsAvailable = true,
                            Services = GetDoctorServices(s.Id)
                        })
                        .ToListAsync();

                    return new DoctorOnDutyResponse { Doctors = fallbackDoctors };
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Error in fallback doctor retrieval");
                    return new DoctorOnDutyResponse { Doctors = new List<DoctorOnDutyDto>() };
                }
            }
        }

        // Step 3: Get available time slots for a specific doctor on a specific date
        public async Task<AvailableSlotsByDoctorResponse> GetAvailableSlotsByDoctorAsync(Guid doctorId, DateTime date, Guid? serviceId = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                var doctorSchedule = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(ds => ds.DoctorId == doctorId && ds.DayOfWeek == dayOfWeek && ds.IsActive);

                // If no specific schedule found, use default working hours
                var shiftStart = doctorSchedule?.StartTime ?? new TimeSpan(9, 0, 0); // Default 9 AM
                var shiftEnd = doctorSchedule?.EndTime ?? new TimeSpan(17, 0, 0); // Default 5 PM
                var breakEnd = (TimeSpan?)null; // ShiftSchedule doesn't have break times

                // Get service duration (default 30 minutes)
                var serviceDuration = 30;
                if (serviceId.HasValue)
                {
                    var service = await _context.Services.FindAsync(serviceId.Value);
                    if (service != null)
                    {
                        serviceDuration = service.DurationMinutes;
                    }
                }

                // Get existing appointments for this doctor on this date (including pending appointments)
                // Also check for any appointments on this date/time that might be unassigned but should block the slot
                var existingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate == date.Date &&
                               (a.Status == "pending" || a.Status == "confirmed" || a.Status == "approved" || a.Status == "scheduled" || a.Status == "walkin"))
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
                        
                        // Check if the time slot overlaps with the appointment
                        // Two time ranges overlap if: start1 < end2 AND start2 < end1
                        if (currentTime < appointmentEnd && appointmentStart < endTime)
                        {
                            isAvailable = false;
                            break; // No need to check other appointments
                        }
                    }

                    // Check if slot is during break time (no break times in ShiftSchedule)
                    // Skip break time check since ShiftSchedule doesn't have break times

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

                return new AvailableSlotsByDoctorResponse
                {
                    DoctorId = doctorId,
                    Date = date,
                    AvailableSlots = slots.Where(s => s.IsAvailable).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available slots for doctor: {DoctorId} on date: {Date}", doctorId, date);
                
                // Return empty slots if there's an error
                return new AvailableSlotsByDoctorResponse
                {
                    DoctorId = doctorId,
                    Date = date,
                    AvailableSlots = new List<TimeSlotDto>()
                };
            }
        }

        // Step 4: Get staff on duty
        public async Task<StaffOnDutyResponse> GetStaffOnDutyAsync(DateTime date, string? role = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                
                // Get all active staff members who are doctors
                var activeStaff = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync();

                // Filter for doctors (assuming doctors have a specific role in User)
                var doctors = activeStaff.Where(s => s.User != null).ToList();

                var staffOnDuty = new List<StaffOnDutyDto>();

                foreach (var staff in doctors)
                {
                    // Check if this staff member has a schedule for the given date
                    var schedule = await _context.ShiftSchedules
                        .FirstOrDefaultAsync(ds => ds.DoctorId == staff.Id && 
                                                  ds.DayOfWeek == dayOfWeek && 
                                                  ds.IsActive);

                    if (schedule != null)
                    {
                        staffOnDuty.Add(new StaffOnDutyDto
                        {
                            Id = staff.Id,
                            FirstName = staff.User?.FirstName ?? "Unknown",
                            LastName = staff.User?.LastName ?? "Unknown",
                            Role = "doctor",
                            Specialty = "General",
                        ShiftStart = schedule.StartTime,
                        ShiftEnd = schedule.EndTime,
                            IsAvailable = true
                        });
                    }
                }

                return new StaffOnDutyResponse { Staff = staffOnDuty };
            }
            catch (Exception ex)
            {
                // Log the error and return empty list
                Console.WriteLine($"Error in GetStaffOnDutyAsync: {ex.Message}");
                return new StaffOnDutyResponse { Staff = new List<StaffOnDutyDto>() };
            }
        }

        // Step 5: Create appointment with enhanced validation
        public async Task<AppointmentBookingResponse> CreateAppointmentAsync(CreateBookingAppointmentRequest request)
        {
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
                .FirstOrDefaultAsync(a => a.StaffId == request.DoctorId &&
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
        private List<DateTime> GetNextWeekdayDates(DateTime startDate, int count)
        {
            var dates = new List<DateTime>();
            var currentDate = startDate.AddDays(1);

            while (dates.Count < count)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    dates.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            return dates;
        }

        private static List<ServiceDto> GetDoctorServices(Guid doctorId)
        {
            // This could be enhanced to get actual services from a doctor-services relationship
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
