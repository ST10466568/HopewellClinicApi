using System.ComponentModel.DataAnnotations;
using HopewellClinicApi.Models;

namespace HopewellClinicApi.DTOs
{
    // Doctor Shift DTOs
    public class DoctorShiftResponse
    {
        public int Id { get; set; }
        public Guid DoctorId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateDoctorShiftRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Day of week must be between 0 (Sunday) and 6 (Saturday)")]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateDoctorShiftRequest
    {
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }

    // Appointment Approval DTOs
    public class ApproveAppointmentRequest
    {
        public string? Notes { get; set; }
    }

    public class RejectAppointmentRequest
    {
        [Required]
        public string RejectionReason { get; set; } = string.Empty;
    }

    // Enhanced Appointment Response with Approval Info
    public class AppointmentWithApprovalResponse
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
        public string BookingType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Related entities
        public ServiceResponse Service { get; set; } = null!;
        public PatientResponse Patient { get; set; } = null!;
        public StaffResponse? Staff { get; set; }
    }

    // Doctor Schedule DTOs
    public class DoctorScheduleResponse
    {
        public DateTime Date { get; set; }
        public List<AppointmentWithApprovalResponse> Appointments { get; set; } = new();
        public List<object> Shifts { get; set; } = new();
    }

    // Patient Details for Doctors
    public class PatientDetailsForDoctorResponse
    {
        public Guid Id { get; set; }
        public string PatientNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Medical history (appointments)
        public List<AppointmentWithApprovalResponse> AppointmentHistory { get; set; } = new();
    }

    // Doctor Availability DTOs
    public class DoctorAvailabilityResponse
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public List<DoctorShiftResponse> AvailableShifts { get; set; } = new();
        public bool IsAvailableOnDate { get; set; }
    }
}
