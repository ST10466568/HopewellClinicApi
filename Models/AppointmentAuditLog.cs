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
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty; // 'created', 'updated', 'deleted'

        [Required]
        [Column("changed_by")]
        public Guid ChangedBy { get; set; }

        [Required]
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [Column("old_values")]
        public string? OldValues { get; set; } // JSON of old values

        [Column("new_values")]
        public string? NewValues { get; set; } // JSON of new values

        // Navigation properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;

        [ForeignKey("ChangedBy")]
        public virtual Staff ChangedByStaff { get; set; } = null!;
    }
}
