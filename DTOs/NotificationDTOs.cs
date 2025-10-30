using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    public class SendCustomEmailRequest
    {
        [Required(ErrorMessage = "PatientId is required")]
        public Guid PatientId { get; set; }
        
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject must be between 1 and 200 characters")]
        public string Subject { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Message is required")]
        [StringLength(5000, ErrorMessage = "Message must be between 1 and 5000 characters")]
        public string Message { get; set; } = string.Empty;
        
        public string? TemplateId { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class SendBulkEmailRequest
    {
        public List<Guid> PatientIds { get; set; } = new();
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? TemplateId { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class NotificationHistoryFilters
    {
        public Guid? PatientId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? HasAppointment { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ToggleSettingsRequest
    {
        public string SettingName { get; set; } = string.Empty;
        public bool Value { get; set; }
    }

    public class PreviewEmailRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? TemplateId { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
    }

    // New preview request matching spec format
    public class PreviewTemplateRequest
    {
        [Required]
        public string TemplateType { get; set; } = string.Empty;
        
        public Dictionary<string, object>? Data { get; set; }
    }

    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string? MessageId { get; set; }
        public string? PreviewUrl { get; set; }
        public string? Error { get; set; }
    }

    public class EmailPreviewResult
    {
        public string Subject { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
    }

    public class EmailTestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class BulkEmailResult
    {
        public List<EmailSendResult> Results { get; set; } = new();
        public int TotalSent { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
    }

    public class NotificationHistoryItem
    {
        public Guid Id { get; set; }
        public Guid? PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public Guid? AppointmentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationResponse
    {
        public bool Success { get; set; }
        public string? MessageId { get; set; }
        public string? NotificationId { get; set; }
        public string? PreviewUrl { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
    }

    public class BulkNotificationResponse
    {
        public bool Success { get; set; }
        public List<NotificationResponse> Results { get; set; } = new();
        public int TotalSent { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
    }

    public class NotificationHistoryRequest
    {
        public Guid? PatientId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? HasAppointment { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PatientNotificationHistoryResponse
    {
        public List<NotificationHistoryItem> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class NotificationSettingsResponse
    {
        public Guid Id { get; set; }
        public bool AutoReminder24h { get; set; }
        public bool AutoReminder2h { get; set; }
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool AppointmentConfirmations { get; set; }
        public bool PrescriptionAlerts { get; set; }
        public bool TestResultAlerts { get; set; }
        public bool InsuranceReminders { get; set; }
        public string? ClinicEmail { get; set; }
        public string? ClinicPhone { get; set; }
        public string? ClinicAddress { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateNotificationSettingsRequest
    {
        public bool? Reminder24hEnabled { get; set; }
        public bool? Reminder2hEnabled { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? SmsNotifications { get; set; }
        public bool? AppointmentConfirmations { get; set; }
        public bool? PrescriptionAlerts { get; set; }
        public bool? TestResultAlerts { get; set; }
        public bool? InsuranceReminders { get; set; }
        public string? ClinicEmail { get; set; }
        public string? ClinicPhone { get; set; }
        public string? ClinicAddress { get; set; }
    }

    public class EmailPreviewResponse
    {
        public string Subject { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
    }

    public class EmailConfigurationTestResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class EmailPreviewData
    {
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, object> Variables { get; set; } = new();
    }

    public class PatientPreviewData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AppointmentPreviewData
    {
        public DateTime AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
    }

    public class ServicePreviewData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }

    public class StaffPreviewData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ClinicPreviewData
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Notification Reply DTOs
    public class ReplyNotificationRequest
    {
        [Required]
        public Guid SenderId { get; set; }
        
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SenderRole { get; set; } = string.Empty;
    }

    public class NotificationReplyResponse
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid ThreadId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    // Direct Messaging DTOs
    public class SendMessageRequest
    {
        [Required]
        public Guid RecipientId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RecipientRole { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public Guid SenderId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string SenderRole { get; set; } = string.Empty;
    }

    public class SendMessageResponse
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid ThreadId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    // Thread DTOs
    public class ThreadParticipantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ThreadMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class ThreadResponse
    {
        public Guid ThreadId { get; set; }
        public List<ThreadParticipantDto> Participants { get; set; } = new();
        public List<ThreadMessageDto> Messages { get; set; } = new();
    }

    // Push Notification DTOs
    public class PushSubscriptionRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UserRole { get; set; } = string.Empty;
        
        [Required]
        public string Subscription { get; set; } = string.Empty; // JSON stringified PushSubscription object
    }

    public class PushSubscriptionResponse
    {
        public bool Success { get; set; }
        public Guid SubscriptionId { get; set; }
    }

    public class SendPushNotificationRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        [StringLength(50)]
        public string? UserRole { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Body { get; set; } = string.Empty;
        
        public Dictionary<string, object>? Data { get; set; }
    }

    public class SendPushNotificationResponse
    {
        public bool Success { get; set; }
        public string? MessageId { get; set; }
    }

    // Enhanced Notification DTOs
    public class EnhancedNotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
        public string EmailSubject { get; set; } = string.Empty;
        public string EmailContent { get; set; } = string.Empty;
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? ServiceName { get; set; }
        public Guid? PatientId { get; set; }
        public string? PatientName { get; set; }
        public Guid? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderRole { get; set; }
        public bool IsRead { get; set; }
        public Guid? ThreadId { get; set; }
        public List<NotificationReplyResponse> Replies { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    // Notification list response with pagination
    public class NotificationListResponse
    {
        public List<EnhancedNotificationDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    // Mark notification as read response
    public class MarkNotificationReadResponse
    {
        public bool Success { get; set; }
        public Guid NotificationId { get; set; }
    }
}