using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopewellClinicApi.Models
{
    /// <summary>
    /// Types of notifications
    /// </summary>
    public enum NotificationType
    {
        Reminder24h,
        Reminder2h,
        Custom,
        AppointmentConfirmation,
        AppointmentCancellation,
        AppointmentReschedule,
        DirectMessage
    }

    /// <summary>
    /// Status of notifications
    /// </summary>
    public enum NotificationStatus
    {
        Scheduled,
        Processing,
        Sent,
        Failed,
        Cancelled
    }
    /// <summary>
    /// Represents a notification record in the system
    /// </summary>
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the appointment (nullable for custom notifications)
        /// </summary>
        public Guid? AppointmentId { get; set; }

        /// <summary>
        /// Reference to the patient/user (nullable for system notifications)
        /// Note: For staff-to-staff messages, this stores the recipient's UserId
        /// </summary>
        public Guid? PatientId { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        [Required]
        public NotificationType Type { get; set; }

        /// <summary>
        /// Current status of the notification
        /// </summary>
        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Scheduled;

        /// <summary>
        /// When the notification should be sent (nullable for immediate sending)
        /// </summary>
        public DateTime? ScheduledFor { get; set; }

        /// <summary>
        /// When the notification was actually sent
        /// </summary>
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// Email subject line (nullable for non-email notifications)
        /// </summary>
        [StringLength(200)]
        public string? EmailSubject { get; set; }

        /// <summary>
        /// Email body content (nullable for non-email notifications)
        /// </summary>
        public string? EmailContent { get; set; }

        /// <summary>
        /// Error message if sending failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// When the record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID of the user who sent the notification (for direct messages)
        /// </summary>
        public Guid? SenderId { get; set; }

        /// <summary>
        /// Name of the sender
        /// </summary>
        [StringLength(255)]
        public string? SenderName { get; set; }

        /// <summary>
        /// Role of the sender (patient, doctor, nurse, admin)
        /// </summary>
        [StringLength(50)]
        public string? SenderRole { get; set; }

        /// <summary>
        /// Whether the notification has been read
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Thread ID for grouping related messages/replies
        /// </summary>
        public Guid? ThreadId { get; set; }

        /// <summary>
        /// Service name (for appointment-related notifications)
        /// </summary>
        [StringLength(200)]
        public string? ServiceName { get; set; }

        // Navigation properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }

        [ForeignKey("PatientId")]
        public virtual ApplicationUser? Patient { get; set; }

        [ForeignKey("SenderId")]
        public virtual ApplicationUser? Sender { get; set; }

        // Navigation property for replies
        public virtual ICollection<NotificationReply> Replies { get; set; } = new List<NotificationReply>();
    }

    /// <summary>
    /// Represents notification settings for the clinic
    /// </summary>
    public class NotificationSettings
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Whether 24-hour reminders are enabled
        /// </summary>
        public bool AutoReminder24h { get; set; } = true;

        /// <summary>
        /// Whether 2-hour reminders are enabled
        /// </summary>
        public bool AutoReminder2h { get; set; } = true;

        /// <summary>
        /// Whether email notifications are enabled
        /// </summary>
        public bool EmailNotifications { get; set; } = true;

        /// <summary>
        /// Whether SMS notifications are enabled
        /// </summary>
        public bool SmsNotifications { get; set; } = false;

        /// <summary>
        /// Whether appointment confirmations are enabled
        /// </summary>
        public bool AppointmentConfirmations { get; set; } = true;

        /// <summary>
        /// Whether prescription alerts are enabled
        /// </summary>
        public bool PrescriptionAlerts { get; set; } = true;

        /// <summary>
        /// Whether test result alerts are enabled
        /// </summary>
        public bool TestResultAlerts { get; set; } = true;

        /// <summary>
        /// Whether insurance reminders are enabled
        /// </summary>
        public bool InsuranceReminders { get; set; } = true;

        /// <summary>
        /// Clinic email address
        /// </summary>
        [StringLength(255)]
        public string? ClinicEmail { get; set; }

        /// <summary>
        /// Clinic phone number
        /// </summary>
        [StringLength(50)]
        public string? ClinicPhone { get; set; }

        /// <summary>
        /// Clinic physical address
        /// </summary>
        public string? ClinicAddress { get; set; }

        /// <summary>
        /// When the settings were created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the settings were last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
