using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopewellClinicApi.Models
{
    /// <summary>
    /// Represents a reply to a notification/message
    /// </summary>
    public class NotificationReply
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the parent notification
        /// </summary>
        [Required]
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Thread ID for grouping related messages
        /// </summary>
        [Required]
        public Guid ThreadId { get; set; }

        /// <summary>
        /// ID of the user who sent the reply
        /// </summary>
        [Required]
        public Guid SenderId { get; set; }

        /// <summary>
        /// Name of the sender
        /// </summary>
        [Required]
        [StringLength(255)]
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Role of the sender (patient, doctor, nurse, admin)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SenderRole { get; set; } = string.Empty;

        /// <summary>
        /// Content of the reply
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// When the reply was sent
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the reply has been read
        /// </summary>
        public bool IsRead { get; set; } = false;

        // Navigation properties
        [ForeignKey("NotificationId")]
        public virtual Notification Notification { get; set; } = null!;

        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; } = null!;
    }
}

