using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Models;

namespace HopewellClinicApi.Data
{
    public static class DoctorScheduleSeeder
    {
        public static async Task SeedDoctorSchedulesAsync(HopewellDbContext context)
        {
            // Get all active staff members who are doctors
            var doctors = await context.Staff
                .Where(s => s.IsActive)
                .ToListAsync();

            if (!doctors.Any())
            {
                return; // No doctors to seed schedules for
            }

            var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var schedules = new List<ShiftSchedule>();

            foreach (var doctor in doctors)
            {
                // Check if doctor already has schedules
                var existingSchedules = await context.ShiftSchedules
                    .Where(ds => ds.DoctorId == doctor.Id)
                    .AnyAsync();

                if (existingSchedules)
                {
                    continue; // Doctor already has schedules
                }

                foreach (var dayOfWeek in daysOfWeek)
                {
                    var isWeekend = dayOfWeek == "Saturday" || dayOfWeek == "Sunday";
                    
                    var schedule = new ShiftSchedule
                    {
                        Id = Guid.NewGuid(),
                        DoctorId = doctor.Id,
                        DayOfWeek = dayOfWeek,
                        IsActive = !isWeekend, // Weekends are inactive by default
                        StartTime = isWeekend ? TimeSpan.Zero : new TimeSpan(9, 0, 0), // 9:00 AM
                        EndTime = isWeekend ? TimeSpan.Zero : new TimeSpan(17, 0, 0), // 5:00 PM
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    schedules.Add(schedule);
                }
            }

            if (schedules.Any())
            {
                context.ShiftSchedules.AddRange(schedules);
                await context.SaveChangesAsync();
            }
        }
    }
}

