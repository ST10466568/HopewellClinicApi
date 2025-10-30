using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopewellClinicApi.Models
{
    /// <summary>
    /// Represents a push notification subscription for a user
    /// </summary>
    public class PushSubscription
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the user who subscribed
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Role of the user (patient, doctor, nurse, admin)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// Push notification endpoint URL
        /// </summary>
        [Required]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// P256dh key for encryption
        /// </summary>
        [Required]
        public string P256dhKey { get; set; } = string.Empty;

        /// <summary>
        /// Auth key for encryption
        /// </summary>
        [Required]
        public string AuthKey { get; set; } = string.Empty;

        /// <summary>
        /// When the subscription was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the subscription is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}

