using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    public class CreateAppointmentRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;

        public string EndTime { get; set; } = string.Empty;

        public Guid? StaffId { get; set; }

        public string? Notes { get; set; }
    }

    public class UpdateAppointmentRequest
    {
        public DateTime? AppointmentDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? StaffId { get; set; }
    }

    public class AssignStaffRequest
    {
        [Required]
        public Guid StaffId { get; set; }
    }

     public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public ServiceResponse? Service { get; set; }
        public PatientResponse? Patient { get; set; }
        public StaffResponse? Staff { get; set; }
    }



    public class TimeSlotResponse
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}