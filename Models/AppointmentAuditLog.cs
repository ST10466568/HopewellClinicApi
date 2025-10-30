using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopewellClinicApi.Models
{
    [Table("AppointmentAuditLog")]
    public class AppointmentAuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("appointment_id")]
        public Guid AppointmentId { get; set; }

        [Required]
        [Column("action")]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // 'created', 'updated', 'approved', 'rejected', 'cancelled'

        [Column("old_status")]
        [MaxLength(50)]
        public string? OldStatus { get; set; }

        [Column("new_status")]
        [MaxLength(50)]
        public string? NewStatus { get; set; }

        [Column("reason")]
        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        [Column("performed_by")]
        public Guid PerformedBy { get; set; }

        [Required]
        [Column("performed_at")]
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        [Column("details")]
        [MaxLength(1000)]
        public string? Details { get; set; }

        // Navigation properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;

        [ForeignKey("PerformedBy")]
        public virtual ApplicationUser PerformedByUser { get; set; } = null!;
    }
}
