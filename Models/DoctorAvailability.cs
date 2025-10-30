using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopewellClinicApi.Models
{
    [Table("doctor_availability")]
    public class DoctorAvailability
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("doctor_id")]
        public Guid DoctorId { get; set; }

        [Required]
        [Column("date")]
        public DateTime Date { get; set; }

        [Column("total_slots")]
        public int TotalSlots { get; set; } = 0;

        [Column("booked_slots")]
        public int BookedSlots { get; set; } = 0;

        [Column("available_slots")]
        public int AvailableSlots { get; set; }

        [Column("is_fully_booked")]
        public bool IsFullyBooked { get; set; }

        [Column("is_on_duty")]
        public bool IsOnDuty { get; set; } = true;

        [Column("unavailability_reason")]
        [MaxLength(255)]
        public string? UnavailabilityReason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("DoctorId")]
        public virtual Staff Doctor { get; set; } = null!;
    }
}
