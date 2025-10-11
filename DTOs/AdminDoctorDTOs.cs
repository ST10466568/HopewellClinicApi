using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    // DTO for doctor information in admin interface
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? StaffNumber { get; set; }
    }

    // DTO for shift schedule data
    public class ShiftScheduleDto
    {
        [Required]
        public string DayOfWeek { get; set; } = string.Empty;
        
        [Required]
        public string StartTime { get; set; } = string.Empty; // Format: "HH:mm"
        
        [Required]
        public string EndTime { get; set; } = string.Empty; // Format: "HH:mm"
        
        [Required]
        public bool IsActive { get; set; }
    }

    // Request DTO for updating doctor shift schedule
    public class UpdateDoctorShiftScheduleRequest
    {
        [Required]
        public List<ShiftScheduleDto> Shifts { get; set; } = new();
    }

    // Response DTO for shift schedule operations
    public class ShiftScheduleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ShiftScheduleDto>? UpdatedShifts { get; set; }
        public string? Error { get; set; }
    }

    // DTO for admin doctor list response
    public class AdminDoctorListResponse
    {
        public bool Success { get; set; }
        public List<DoctorDto> Doctors { get; set; } = new();
        public int TotalCount { get; set; }
        public string? Error { get; set; }
    }

    // DTO for doctor schedule response
    public class AdminDoctorScheduleResponse
    {
        public bool Success { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public List<ShiftScheduleDto> Schedule { get; set; } = new();
        public string? Error { get; set; }
    }
}
