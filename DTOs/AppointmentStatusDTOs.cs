using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    /// <summary>
    /// Request DTO for updating appointment status
    /// </summary>
    public class UpdateAppointmentStatusRequest
    {
        [Required]
        [RegularExpression("^(pending|confirmed|cancelled|completed|walkin)$", 
            ErrorMessage = "Invalid status. Must be one of: pending, confirmed, cancelled, completed, walkin")]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Response DTO for appointment status updates
    /// </summary>
    public class AppointmentStatusResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for appointment approval
    /// </summary>
    public class ApproveAppointmentRequest
    {
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request DTO for appointment rejection
    /// </summary>
    public class RejectAppointmentRequest
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AdditionalNotes { get; set; }
    }

    /// <summary>
    /// Response DTO for appointment approval/rejection
    /// </summary>
    public class AppointmentActionResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // "approved" or "rejected"
        public string Message { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string? Reason { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Audit log entry for appointment changes
    /// </summary>
    public class AppointmentAuditLogEntry
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Reason { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? Details { get; set; }
    }
}











