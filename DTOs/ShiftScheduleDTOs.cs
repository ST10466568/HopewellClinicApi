using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    // Doctor availability result with shift schedule validation
    public class DoctorAvailabilityResult
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public bool IsOnDuty { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFullyBooked { get; set; }
        public int AvailableSlots { get; set; }
        public int TotalSlots { get; set; }
        public string? UnavailabilityReason { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public DoctorShiftInfo? ShiftSchedule { get; set; }
    }

    // Doctor shift information
    public class DoctorShiftInfo
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? BreakStartTime { get; set; }
        public string? BreakEndTime { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }

    // Enhanced doctor with shift schedule validation
    public class DoctorWithShiftValidation
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string ShiftStart { get; set; } = string.Empty;
        public string ShiftEnd { get; set; } = string.Empty;
        public bool IsOnDuty { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFullyBooked { get; set; }
        public int AvailableSlots { get; set; }
        public int TotalSlots { get; set; }
        public string? UnavailabilityReason { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public List<ServiceDto> Services { get; set; } = new();
    }

    // Enhanced doctors response with shift validation
    public class DoctorsWithShiftValidationResponse
    {
        public List<DoctorWithShiftValidation> Doctors { get; set; } = new();
        public DateTime RequestedDate { get; set; }
        public DoctorAvailabilitySummary Summary { get; set; } = new();
    }

    // Enhanced availability summary with shift validation (extends existing DoctorAvailabilitySummary)
    public class EnhancedDoctorAvailabilitySummary : DoctorAvailabilitySummary
    {
        public int OnDutyDoctors { get; set; }
        public int OffDutyDoctors { get; set; }
    }

    // Doctor shift schedule response
    public class DoctorShiftScheduleResponse
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public List<DoctorShiftInfo> Shifts { get; set; } = new();
        public DateTime? NextAvailableDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // Doctor availability check response
    public class DoctorAvailabilityCheckResponse
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public bool IsOnDuty { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; }
        public DoctorShiftInfo? ShiftSchedule { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public List<AlternativeDoctor> AlternativeDoctors { get; set; } = new();
    }

    // Alternative doctor suggestion
    public class AlternativeDoctor
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
        public double Rating { get; set; }
    }

    // Request DTO for updating shift schedules
    public class UpdateShiftScheduleRequest
    {
        public List<DoctorShiftInfo> Shifts { get; set; } = new();
    }

    // Shift schedule item for frontend compatibility (matches frontend ShiftSchedule interface)
    public class ShiftScheduleItem
    {
        public Guid? Id { get; set; }
        
        [Required]
        public string DayOfWeek { get; set; } = string.Empty; // "Monday", "Tuesday", etc.
        
        [Required]
        public string StartTime { get; set; } = string.Empty; // "HH:mm" format
        
        [Required]
        public string EndTime { get; set; } = string.Empty; // "HH:mm" format
        
        public bool IsActive { get; set; } = true;
    }

    // Booking validation result
    public class BookingValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public DoctorAvailabilityResult? AvailabilityInfo { get; set; }
    }
}
