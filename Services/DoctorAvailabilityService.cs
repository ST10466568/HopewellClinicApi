using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly HopewellDbContext _context;
        private readonly ILogger<DoctorAvailabilityService> _logger;

        public DoctorAvailabilityService(HopewellDbContext context, ILogger<DoctorAvailabilityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DoctorAvailabilitySummary> GetAvailabilitySummaryAsync(DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                
                // Get all active doctors
                var activeDoctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync();

                var totalDoctors = activeDoctors.Count;
                var availableDoctors = 0;
                var fullyBookedDoctors = 0;
                var totalAvailableSlots = 0;
                var totalBookedSlots = 0;

                foreach (var doctor in activeDoctors)
                {
                    var availability = await GetDoctorAvailabilityAsync(doctor.Id, date);
                    if (availability != null)
                    {
                        if (availability.IsFullyBooked)
                        {
                            fullyBookedDoctors++;
                        }
                        else
                        {
                            availableDoctors++;
                        }
                        totalAvailableSlots += availability.AvailableSlots;
                        totalBookedSlots += availability.BookedSlots;
                    }
                }

                return new DoctorAvailabilitySummary
                {
                    Date = date,
                    TotalDoctors = totalDoctors,
                    AvailableDoctors = availableDoctors,
                    FullyBookedDoctors = fullyBookedDoctors,
                    TotalAvailableSlots = totalAvailableSlots,
                    TotalBookedSlots = totalBookedSlots
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability summary for date: {Date}", date);
                return new DoctorAvailabilitySummary { Date = date };
            }
        }

        public async Task<List<DoctorWithAvailability>> GetDoctorsWithAvailabilityAsync(DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                
                var doctors = await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync();

                var result = new List<DoctorWithAvailability>();

                foreach (var doctor in doctors)
                {
                    var availability = await GetDoctorAvailabilityAsync(doctor.Id, date);
                    var nextAvailableDate = availability?.IsFullyBooked == true 
                        ? await GetNextAvailableDateAsync(doctor.Id, date) 
                        : null;

                    result.Add(new DoctorWithAvailability
                    {
                        Id = doctor.Id,
                        Name = $"{doctor.User?.FirstName} {doctor.User?.LastName}",
                        Specialty = "General Practice", // This could be enhanced to get actual specialty
                        IsFullyBooked = availability?.IsFullyBooked ?? false,
                        AvailableSlots = availability?.AvailableSlots ?? 0,
                        TotalSlots = availability?.TotalSlots ?? 0,
                        NextAvailableDate = nextAvailableDate
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors with availability for date: {Date}", date);
                return new List<DoctorWithAvailability>();
            }
        }

        public async Task<DoctorAvailability?> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date)
        {
            try
            {
                var availability = await _context.DoctorAvailability
                    .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.Date == date.Date);

                if (availability == null)
                {
                    // Calculate and create availability record
                    await UpdateDoctorAvailabilityAsync(doctorId, date);
                    availability = await _context.DoctorAvailability
                        .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.Date == date.Date);
                }

                return availability;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor availability for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return null;
            }
        }

        public async Task<DoctorAvailabilityResult> CheckDoctorAvailabilityAsync(Guid doctorId, DateTime date)
        {
            try
            {
                // 1. Check if doctor is on duty (shift schedule)
                var isOnDuty = await IsDoctorOnDutyAsync(doctorId, date);
                
                if (!isOnDuty)
                {
                    var nextAvailableDate = await GetNextAvailableDateAsync(doctorId, date);
                    return new DoctorAvailabilityResult
                    {
                        DoctorId = doctorId,
                        Date = date,
                        IsOnDuty = false,
                        IsAvailable = false,
                        IsFullyBooked = false,
                        AvailableSlots = 0,
                        TotalSlots = 0,
                        UnavailabilityReason = "Not scheduled to work on this day",
                        NextAvailableDate = nextAvailableDate
                    };
                }

                // 2. Get shift schedule info
                var shiftInfo = await GetDoctorShiftInfoAsync(doctorId, date);

                // 3. Check time slot availability
                var availability = await GetDoctorAvailabilityAsync(doctorId, date);
                var isFullyBooked = availability?.IsFullyBooked ?? false;
                var availableSlots = availability?.AvailableSlots ?? 0;
                var totalSlots = availability?.TotalSlots ?? 0;

                return new DoctorAvailabilityResult
                {
                    DoctorId = doctorId,
                    Date = date,
                    IsOnDuty = true,
                    IsAvailable = !isFullyBooked,
                    IsFullyBooked = isFullyBooked,
                    AvailableSlots = availableSlots,
                    TotalSlots = totalSlots,
                    UnavailabilityReason = isFullyBooked ? "All time slots are booked" : null,
                    NextAvailableDate = isFullyBooked ? await GetNextAvailableDateAsync(doctorId, date) : null,
                    ShiftSchedule = shiftInfo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking doctor availability for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return new DoctorAvailabilityResult
                {
                    DoctorId = doctorId,
                    Date = date,
                    IsOnDuty = false,
                    IsAvailable = false,
                    UnavailabilityReason = "Error checking availability"
                };
            }
        }

        public async Task<bool> IsDoctorOnDutyAsync(Guid doctorId, DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                
                // Check if doctor has an active shift for this day of week
                var shift = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => 
                        s.DoctorId == doctorId && 
                        s.DayOfWeek == dayOfWeek && 
                        s.IsActive == true);
                        
                return shift != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if doctor is on duty for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return false;
            }
        }

        public async Task<DoctorShiftInfo?> GetDoctorShiftInfoAsync(Guid doctorId, DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                var shift = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => 
                        s.DoctorId == doctorId && 
                        s.DayOfWeek == dayOfWeek);

                if (shift == null) return null;

                return new DoctorShiftInfo
                {
                    Id = shift.Id,
                    DayOfWeek = shift.DayOfWeek,
                    StartTime = shift.StartTime.ToString(@"hh\:mm"),
                    EndTime = shift.EndTime.ToString(@"hh\:mm"),
                    IsActive = shift.IsActive,
                    BreakStartTime = null, // ShiftSchedule doesn't have break times
                    BreakEndTime = null,
                    EffectiveFrom = null,
                    EffectiveTo = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor shift info for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return null;
            }
        }

        public async Task UpdateDoctorAvailabilityAsync(Guid doctorId, DateTime date)
        {
            try
            {
                // Check if doctor is on duty first
                var isOnDuty = await IsDoctorOnDutyAsync(doctorId, date);
                
                var totalSlots = 0;
                var bookedSlots = 0;
                var unavailabilityReason = (string?)null;

                if (isOnDuty)
                {
                    totalSlots = await CalculateTotalSlotsAsync(doctorId, date);
                    bookedSlots = await CalculateBookedSlotsAsync(doctorId, date);
                }
                else
                {
                    unavailabilityReason = "Not scheduled to work on this day";
                }

                var availability = await _context.DoctorAvailability
                    .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.Date == date.Date);

                if (availability == null)
                {
                    availability = new DoctorAvailability
                    {
                        DoctorId = doctorId,
                        Date = date.Date,
                        TotalSlots = totalSlots,
                        BookedSlots = bookedSlots,
                        AvailableSlots = totalSlots - bookedSlots,
                        IsFullyBooked = isOnDuty && (totalSlots - bookedSlots) <= 0,
                        IsOnDuty = isOnDuty,
                        UnavailabilityReason = unavailabilityReason,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.DoctorAvailability.Add(availability);
                }
                else
                {
                    availability.TotalSlots = totalSlots;
                    availability.BookedSlots = bookedSlots;
                    availability.AvailableSlots = totalSlots - bookedSlots;
                    availability.IsFullyBooked = isOnDuty && (totalSlots - bookedSlots) <= 0;
                    availability.IsOnDuty = isOnDuty;
                    availability.UnavailabilityReason = unavailabilityReason;
                    availability.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor availability for doctor: {DoctorId}, date: {Date}", doctorId, date);
            }
        }

        public async Task<List<DateTime>> GetNextAvailableDatesAsync(Guid doctorId, int maxDays = 7)
        {
            try
            {
                var availableDates = new List<DateTime>();
                var currentDate = DateTime.Today.AddDays(1); // Start from tomorrow
                var endDate = currentDate.AddDays(maxDays);

                for (var date = currentDate; date < endDate; date = date.AddDays(1))
                {
                    var availability = await GetDoctorAvailabilityAsync(doctorId, date);
                    if (availability != null && !availability.IsFullyBooked)
                    {
                        availableDates.Add(date);
                    }
                }

                return availableDates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available dates for doctor: {DoctorId}", doctorId);
                return new List<DateTime>();
            }
        }

        public async Task<int> CalculateTotalSlotsAsync(Guid doctorId, DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek.ToString();
                var schedule = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(ss => ss.DoctorId == doctorId && ss.DayOfWeek == dayOfWeek && ss.IsActive);

                if (schedule == null)
                {
                    // Default schedule: 9 AM to 5 PM with 30-minute slots
                    var defaultStart = new TimeSpan(9, 0, 0);
                    var defaultEnd = new TimeSpan(17, 0, 0);
                    var slotDuration = 30; // minutes
                    var totalMinutes = (int)(defaultEnd - defaultStart).TotalMinutes;
                    return totalMinutes / slotDuration;
                }

                var shiftDuration = (int)(schedule.EndTime - schedule.StartTime).TotalMinutes;
                var defaultSlotDuration = 30; // Default 30-minute slots
                return shiftDuration / defaultSlotDuration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total slots for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return 0;
            }
        }

        public async Task<int> CalculateBookedSlotsAsync(Guid doctorId, DateTime date)
        {
            try
            {
                var bookedAppointments = await _context.Appointments
                    .Where(a => (a.StaffId == doctorId || a.DoctorId == doctorId) &&
                               a.AppointmentDate == date.Date &&
                               (a.Status == "pending" || a.Status == "confirmed" || 
                                a.Status == "approved" || a.Status == "scheduled" || 
                                a.Status == "walkin"))
                    .CountAsync();

                return bookedAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating booked slots for doctor: {DoctorId}, date: {Date}", doctorId, date);
                return 0;
            }
        }

        public async Task UpdateAvailabilityForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var activeDoctors = await _context.Staff
                    .Where(s => s.IsActive)
                    .Select(s => s.Id)
                    .ToListAsync();

                var tasks = new List<Task>();
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    foreach (var doctorId in activeDoctors)
                    {
                        tasks.Add(UpdateDoctorAvailabilityAsync(doctorId, date));
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for date range: {StartDate} to {EndDate}", startDate, endDate);
            }
        }

        public async Task<Dictionary<Guid, DoctorAvailability>> GetDoctorsAvailabilityAsync(List<Guid> doctorIds, DateTime date)
        {
            try
            {
                var availability = await _context.DoctorAvailability
                    .Where(da => doctorIds.Contains(da.DoctorId) && da.Date == date.Date)
                    .ToDictionaryAsync(da => da.DoctorId, da => da);

                // For doctors without availability records, create them
                var missingDoctors = doctorIds.Where(id => !availability.ContainsKey(id)).ToList();
                foreach (var doctorId in missingDoctors)
                {
                    await UpdateDoctorAvailabilityAsync(doctorId, date);
                    var newAvailability = await _context.DoctorAvailability
                        .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.Date == date.Date);
                    if (newAvailability != null)
                    {
                        availability[doctorId] = newAvailability;
                    }
                }

                return availability;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors availability for date: {Date}", date);
                return new Dictionary<Guid, DoctorAvailability>();
            }
        }

        private async Task<DateTime?> GetNextAvailableDateAsync(Guid doctorId, DateTime fromDate)
        {
            try
            {
                var nextAvailableDates = await GetNextAvailableDatesAsync(doctorId, 14); // Look ahead 2 weeks
                return nextAvailableDates.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available date for doctor: {DoctorId}", doctorId);
                return null;
            }
        }
    }
}
