using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Models;
using Microsoft.AspNetCore.Identity;

namespace HopewellClinicApi.Services
{
    public interface IAdminDoctorService
    {
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<List<ShiftScheduleDto>> GetDoctorShiftScheduleAsync(Guid doctorId);
        Task<bool> UpdateDoctorShiftScheduleAsync(Guid doctorId, List<ShiftScheduleDto> shiftData);
        Task<bool> CanManageDoctorScheduleAsync(Guid adminId, Guid doctorId);
        Task<bool> DoctorExistsAsync(Guid doctorId);
    }

    public class AdminDoctorService : IAdminDoctorService
    {
        private readonly HopewellDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDoctorService(HopewellDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Staff
                .Include(s => s.User)
                .Where(s => s.IsActive)
                .Select(s => new DoctorDto
                {
                    Id = s.Id,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    Email = s.User.Email ?? string.Empty,
                    Role = "doctor", // Assuming all staff are doctors for now
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    StaffNumber = s.StaffNumber
                })
                .ToListAsync();

            return doctors;
        }

        public async Task<List<ShiftScheduleDto>> GetDoctorShiftScheduleAsync(Guid doctorId)
        {
            var schedules = await _context.ShiftSchedules
                .Where(ss => ss.DoctorId == doctorId)
                .OrderBy(ss => ss.DayOfWeek)
                .Select(ss => new ShiftScheduleDto
                {
                    DayOfWeek = ss.DayOfWeek,
                    StartTime = ss.StartTime.ToString(@"hh\:mm"),
                    EndTime = ss.EndTime.ToString(@"hh\:mm"),
                    IsActive = ss.IsActive
                })
                .ToListAsync();

            // If no schedules exist, return default schedule
            if (!schedules.Any())
            {
                return GetDefaultSchedule();
            }

            return schedules;
        }

        public async Task<bool> UpdateDoctorShiftScheduleAsync(Guid doctorId, List<ShiftScheduleDto> shiftData)
        {
            try
            {
                // Validate doctor exists
                var doctorExists = await DoctorExistsAsync(doctorId);
                if (!doctorExists)
                {
                    return false;
                }

                // Remove existing schedules
                var existingSchedules = await _context.ShiftSchedules
                    .Where(ss => ss.DoctorId == doctorId)
                    .ToListAsync();

                _context.ShiftSchedules.RemoveRange(existingSchedules);

                // Add new schedules
                var newSchedules = shiftData.Select(sd => new ShiftSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    DayOfWeek = sd.DayOfWeek,
                    StartTime = TimeSpan.Parse(sd.StartTime),
                    EndTime = TimeSpan.Parse(sd.EndTime),
                    IsActive = sd.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                _context.ShiftSchedules.AddRange(newSchedules);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CanManageDoctorScheduleAsync(Guid adminId, Guid doctorId)
        {
            try
            {
                // Check if admin exists and has admin role
                var admin = await _userManager.FindByIdAsync(adminId.ToString());
                if (admin == null)
                {
                    return false;
                }

                var roles = await _userManager.GetRolesAsync(admin);
                if (!roles.Contains("admin"))
                {
                    return false;
                }

                // Check if doctor exists
                return await DoctorExistsAsync(doctorId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DoctorExistsAsync(Guid doctorId)
        {
            return await _context.Staff
                .Where(s => s.Id == doctorId && s.IsActive)
                .AnyAsync();
        }

        private List<ShiftScheduleDto> GetDefaultSchedule()
        {
            var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            
            return daysOfWeek.Select(day => new ShiftScheduleDto
            {
                DayOfWeek = day,
                StartTime = "09:00",
                EndTime = "17:00",
                IsActive = day != "Saturday" && day != "Sunday" // Weekends inactive by default
            }).ToList();
        }
    }
}
