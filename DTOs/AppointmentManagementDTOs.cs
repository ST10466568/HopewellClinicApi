using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    // Request DTO for updating appointments
    public class AdminUpdateAppointmentRequest
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty; // Format: "HH:mm"

        [Required]
        public string EndTime { get; set; } = string.Empty; // Format: "HH:mm"

        [Required]
        public string Status { get; set; } = string.Empty;

        public Guid? StaffId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        public string? Notes { get; set; }
    }

    // Response DTO for appointment operations
    public class AppointmentOperationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? Id { get; set; }
        public string? Error { get; set; }
    }

    // DTO for appointment details with related data
    public class AppointmentDetailDto
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Patient information
        public Guid PatientId { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;

        // Doctor information
        public Guid? StaffId { get; set; }
        public string? DoctorFirstName { get; set; }
        public string? DoctorLastName { get; set; }
        public string? DoctorEmail { get; set; }

        // Service information
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public decimal? ServicePrice { get; set; }
    }

    // Request DTO for search and pagination
    public class AppointmentSearchRequest
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? ServiceId { get; set; }
    }

    // Response DTO for paginated appointments
    public class PagedAppointmentsResponse
    {
        public bool Success { get; set; }
        public List<AppointmentDetailDto> Appointments { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string? Error { get; set; }
    }

    // Validation result DTO
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    // Audit log DTO
    public class AppointmentAuditLogDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string Action { get; set; } = string.Empty; // 'created', 'updated', 'deleted'
        public Guid ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? OldValues { get; set; } // JSON of old values
        public string? NewValues { get; set; } // JSON of new values
        public string? ChangedByName { get; set; }
    }
}
