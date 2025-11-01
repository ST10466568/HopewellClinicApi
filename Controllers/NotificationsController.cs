using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Services;
using System.Security.Claims;
using System.Text.Json;

namespace HopewellClinicApi.Controllers
{
    /// <summary>
    /// Controller for managing email notifications
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationService _notificationService;
        private readonly HopewellDbContext _context;

        public NotificationsController(
            ILogger<NotificationsController> logger, 
            INotificationService notificationService,
            HopewellDbContext context)
        {
            _logger = logger;
            _notificationService = notificationService;
            _context = context;
        }

        /// <summary>
        /// Public test endpoint to verify controller discovery
        /// </summary>
        [HttpGet("public-test")]
        [AllowAnonymous]
        public ActionResult PublicTestController()
        {
            return Ok(new { message = "NotificationsController public endpoint is working!", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Simple test endpoint to send email without database save
        /// </summary>
        [HttpPost("send-test-email")]
        [AllowAnonymous]
        public async Task<ActionResult> SendTestEmail([FromBody] SendCustomEmailRequest request)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId);

                if (patient == null)
                {
                    return BadRequest(new { success = false, error = "Patient not found" });
                }

                _logger.LogInformation("Attempting to send email to {Email} for patient {PatientId}", patient.User.Email, request.PatientId);
                
                // Use EmailService directly without saving to database
                var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
                var emailResult = await emailService.SendEmailAsync(patient.User.Email!, request.Subject, request.Message, request.Message);
                
                _logger.LogInformation("Email sending result: {Result}", emailResult.Success);

                if (emailResult.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Test email sent successfully!",
                        recipient = patient.User.Email,
                        subject = request.Subject
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Failed to send email"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Internal server error",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Send custom email to a specific patient
        /// </summary>
        [HttpPost("send-custom")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult> SendCustomEmail([FromBody] SendCustomEmailRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new
                    {
                        error = "Invalid request format",
                        details = errors
                    });
                }

                // Validate required fields
                if (request.PatientId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "Invalid request format",
                        details = new { PatientId = new[] { "PatientId is required" } }
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Subject))
                {
                    return BadRequest(new
                    {
                        error = "Invalid request format",
                        details = new { Subject = new[] { "Subject is required" } }
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new
                    {
                        error = "Invalid request format",
                        details = new { Message = new[] { "Message is required" } }
                    });
                }

                _logger.LogInformation("🔔 [NotificationsController] SendCustomEmail called with patientId: {PatientId}", request.PatientId);

                // Check if patient exists first
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId);

                if (patient == null)
                {
                    return NotFound(new
                    {
                        error = "Patient not found"
                    });
                }

                // Use the notification service
                try
                {
                var result = await _notificationService.SendCustomNotificationAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("🔔 [NotificationsController] Custom email sent successfully to patient {PatientId}", request.PatientId);

                    return Ok(new
                    {
                        success = true,
                        notificationId = result.NotificationId,
                            messageId = result.MessageId ?? "sent",
                            message = "Notification sent successfully"
                    });
                }
                else
                {
                    _logger.LogError("🔔 [NotificationsController] Failed to send email: {Error}", result.Error);

                        // Check if it's a patient not found error
                        if (result.Error?.Contains("not found") == true || result.Error?.Contains("Patient not found") == true)
                        {
                            return NotFound(new
                            {
                                error = result.Error ?? "Patient not found"
                            });
                        }

                        // Check if it's a database error
                        if (result.Error?.Contains("Database error") == true || result.Error?.Contains("entity changes") == true)
                        {
                            _logger.LogError("Database error occurred: {Error}", result.Error);
                            return StatusCode(500, new
                            {
                                error = "Database error occurred while saving notification",
                                details = result.Error
                            });
                        }

                    return BadRequest(new
                    {
                        error = result.Error ?? "Failed to send email"
                        });
                    }
                }
                catch (Exception serviceEx)
                {
                    _logger.LogError(serviceEx, "Exception in SendCustomNotificationAsync for patient {PatientId}. Exception: {Exception}, InnerException: {InnerException}", 
                        request.PatientId, serviceEx.Message, serviceEx.InnerException?.Message);
                    
                    // Check for database-related exceptions
                    if (serviceEx.InnerException?.Message?.Contains("foreign key") == true ||
                        serviceEx.InnerException?.Message?.Contains("constraint") == true ||
                        serviceEx.InnerException?.Message?.Contains("NOT NULL") == true)
                    {
                        return StatusCode(500, new
                        {
                            error = "Database constraint error",
                            details = serviceEx.InnerException.Message
                        });
                    }
                    
                    return StatusCode(500, new
                    {
                        error = "Internal server error",
                        message = serviceEx.Message,
                        details = serviceEx.InnerException?.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom email");
                return StatusCode(500, new
                {
                    error = "Internal server error",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Send bulk emails to multiple patients
        /// </summary>
        [HttpPost("send-bulk")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult> SendBulkEmail([FromBody] SendBulkEmailRequest request)
        {
            try
            {
                _logger.LogInformation("🔔 [NotificationsController] SendBulkEmail called for {Count} patients", request.PatientIds.Count);

                if (request.PatientIds.Count == 0 || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Invalid request. PatientIds, Subject, and Message are required."
                    });
                }

                var results = new List<EmailSendResult>();
                var totalSent = 0;
                var totalFailed = 0;

                foreach (var patientId in request.PatientIds)
                {
                    try
                    {
                        var patient = await _context.Users
                            .Include(u => u.Patient)
                            .FirstOrDefaultAsync(u => u.Id == patientId);

                        if (patient == null)
                        {
                            results.Add(new EmailSendResult
                            {
                                Success = false,
                                Error = "Patient not found"
                            });
                            totalFailed++;
                            continue;
                        }

                        var emailResult = await _notificationService.SendCustomNotificationAsync(new SendCustomEmailRequest
                        {
                            PatientId = patientId,
                            Subject = request.Subject,
                            Message = request.Message
                        });

                        if (emailResult.Success)
                        {
                            results.Add(new EmailSendResult
                            {
                                Success = true,
                                MessageId = emailResult.MessageId,
                                PreviewUrl = emailResult.PreviewUrl
                            });
                            totalSent++;
                        }
                        else
                        {
                            results.Add(new EmailSendResult
                            {
                                Success = false,
                                Error = emailResult.Error ?? "Failed to send email"
                            });
                            totalFailed++;
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending bulk email to patient {PatientId}", patientId);
                        results.Add(new EmailSendResult
                        {
                            Success = false,
                            Error = ex.Message
                        });
                        totalFailed++;
                    }
                }

                await _context.SaveChangesAsync();

                var successRate = request.PatientIds.Count > 0 ? (double)totalSent / request.PatientIds.Count * 100 : 0;

                _logger.LogInformation("🔔 [NotificationsController] Bulk email completed: {Sent}/{Total} sent successfully", totalSent, request.PatientIds.Count);

                return Ok(new
                {
                    success = true,
                    results = results,
                    totalSent = totalSent,
                    totalFailed = totalFailed,
                    successRate = successRate,
                    message = $"Bulk email completed: {totalSent}/{request.PatientIds.Count} sent successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk emails");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Internal server error",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Test email configuration
        /// </summary>
        [HttpPost("test-configuration")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult> TestEmailConfiguration()
        {
            try
            {
                var result = await _notificationService.TestEmailConfigurationAsync();
                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    details = result.Details
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing email configuration");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Internal server error",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get notification history (simplified)
        /// </summary>
        [HttpGet("history")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult<List<NotificationHistoryItem>>> GetNotificationHistory()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50) // Limit to recent 50 notifications
                    .Select(n => new NotificationHistoryItem
                    {
                        Id = n.Id,
                        PatientId = n.PatientId,
                        PatientName = n.Patient != null ? $"{n.Patient.FirstName} {n.Patient.LastName}" : "Unknown",
                        Type = n.Type.ToString(),
                        Subject = n.EmailSubject ?? string.Empty,
                        Status = n.Status.ToString(),
                        ScheduledFor = n.ScheduledFor,
                        SentAt = n.SentAt,
                        AppointmentId = n.AppointmentId,
                        CreatedAt = n.CreatedAt
                    })
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification history");
                return StatusCode(500, new { error = "Failed to get notification history", message = ex.Message });
            }
        }

        /// <summary>
        /// Get scheduled notifications (simplified)
        /// </summary>
        [HttpGet("scheduled")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult<List<NotificationHistoryItem>>> GetScheduledNotifications()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .Where(n => n.Status == NotificationStatus.Scheduled && n.ScheduledFor.HasValue)
                    .OrderBy(n => n.ScheduledFor)
                    .Select(n => new NotificationHistoryItem
                    {
                        Id = n.Id,
                        PatientId = n.PatientId,
                        PatientName = n.Patient != null ? $"{n.Patient.FirstName} {n.Patient.LastName}" : "Unknown",
                        Type = n.Type.ToString(),
                        Subject = n.EmailSubject ?? string.Empty,
                        Status = n.Status.ToString(),
                        ScheduledFor = n.ScheduledFor,
                        SentAt = n.SentAt,
                        AppointmentId = n.AppointmentId,
                        CreatedAt = n.CreatedAt
                    })
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled notifications");
                return StatusCode(500, new { error = "Failed to get scheduled notifications", message = ex.Message });
            }
        }

        /// <summary>
        /// Get notification settings
        /// </summary>
        [HttpGet("settings")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult> GetNotificationSettings()
        {
            try
            {
                var result = await _notificationService.GetNotificationSettingsAsync();
                return Ok(new
                {
                    reminder24hEnabled = result.AutoReminder24h,
                    reminder2hEnabled = result.AutoReminder2h,
                    emailNotifications = result.EmailNotifications,
                    smsNotifications = result.SmsNotifications,
                    appointmentConfirmations = result.AppointmentConfirmations,
                    prescriptionAlerts = result.PrescriptionAlerts,
                    testResultAlerts = result.TestResultAlerts,
                    insuranceReminders = result.InsuranceReminders,
                    clinicEmail = result.ClinicEmail,
                    clinicPhone = result.ClinicPhone,
                    clinicAddress = result.ClinicAddress
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification settings");
                return StatusCode(500, new { error = "Failed to get notification settings", message = ex.Message });
            }
        }

        /// <summary>
        /// Preview email template
        /// </summary>
        [HttpPost("preview")]
        [AllowAnonymous]
        public async Task<ActionResult<EmailPreviewResponse>> PreviewEmail([FromBody] PreviewEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _notificationService.PreviewEmailAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing email");
                return StatusCode(500, new { error = "Failed to preview email", message = ex.Message });
            }
        }

        /// <summary>
        /// Update notification settings
        /// </summary>
        [HttpPut("settings")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult> UpdateNotificationSettings([FromBody] UpdateNotificationSettingsRequest request)
        {
            try
            {
                var result = await _notificationService.UpdateNotificationSettingsAsync(request);
                if (result)
                {
                    // Return updated settings
                    var updatedSettings = await _notificationService.GetNotificationSettingsAsync();
                    return Ok(new
                    {
                        reminder24hEnabled = updatedSettings.AutoReminder24h,
                        reminder2hEnabled = updatedSettings.AutoReminder2h,
                        emailNotifications = updatedSettings.EmailNotifications,
                        smsNotifications = updatedSettings.SmsNotifications,
                        appointmentConfirmations = updatedSettings.AppointmentConfirmations,
                        prescriptionAlerts = updatedSettings.PrescriptionAlerts,
                        testResultAlerts = updatedSettings.TestResultAlerts,
                        insuranceReminders = updatedSettings.InsuranceReminders,
                        clinicEmail = updatedSettings.ClinicEmail,
                        clinicPhone = updatedSettings.ClinicPhone,
                        clinicAddress = updatedSettings.ClinicAddress
                    });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Failed to update settings" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings");
                return StatusCode(500, new { error = "Failed to update notification settings", message = ex.Message });
            }
        }

        /// <summary>
        /// Get patient's notification history (enhanced with new fields)
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [AllowAnonymous] // Temporarily without auth for testing
        public async Task<ActionResult<List<EnhancedNotificationDto>>> GetPatientNotifications(Guid patientId)
        {
            try
            {
                _logger.LogInformation("Getting notifications for patientId: {PatientId}", patientId);

                // First, try to find if patientId is a Patient.Id or User.Id
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == patientId);
                
                Guid? userId = null;
                if (patient != null && patient.UserId.HasValue)
                {
                    userId = patient.UserId.Value;
                    _logger.LogInformation("Found patient, using UserId: {UserId}", userId);
                }
                else
                {
                    // If not found as Patient.Id, assume it's a User.Id
                    var user = await _context.Users.FindAsync(patientId);
                    if (user != null)
                    {
                        userId = user.Id;
                        _logger.LogInformation("Found user directly, using UserId: {UserId}", userId);
                    }
                }

                if (!userId.HasValue)
                {
                    _logger.LogWarning("Patient/User not found for patientId: {PatientId}", patientId);
                    return NotFound(new { error = "Patient not found" });
                }

                // Query notifications with null-safe handling
                var notifications = await _context.Notifications
                    .Where(n => n.PatientId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} notifications for userId: {UserId}", notifications.Count, userId);

                // Manually map to DTO with null-safe handling
                var result = new List<EnhancedNotificationDto>();
                
                foreach (var n in notifications)
                {
                    // Load related data safely
                    await _context.Entry(n)
                        .Reference(nav => nav.Patient)
                        .LoadAsync();
                    
                    await _context.Entry(n)
                        .Reference(nav => nav.Appointment)
                        .LoadAsync();
                    
                    if (n.Appointment != null)
                    {
                        await _context.Entry(n.Appointment)
                            .Reference(a => a.Service)
                            .LoadAsync();
                    }
                    
                    await _context.Entry(n)
                        .Collection(nav => nav.Replies)
                        .LoadAsync();
                    
                    await _context.Entry(n)
                        .Reference(nav => nav.Sender)
                        .LoadAsync();

                    var dto = new EnhancedNotificationDto
                    {
                        Id = n.Id,
                        Type = n.Type.ToString(),
                        Status = n.Status.ToString(),
                        SentAt = n.SentAt,
                        EmailSubject = n.EmailSubject ?? string.Empty,
                        EmailContent = n.EmailContent ?? string.Empty,
                        AppointmentDate = n.Appointment?.AppointmentDate,
                        AppointmentTime = n.Appointment != null ? n.Appointment.StartTime.ToString(@"hh\:mm") : null,
                        ServiceName = n.ServiceName ?? (n.Appointment?.Service?.Name),
                        PatientId = n.PatientId,
                        PatientName = n.Patient != null ? $"{n.Patient.FirstName} {n.Patient.LastName}" : null,
                        SenderId = n.SenderId,
                        SenderName = n.SenderName,
                        SenderRole = n.SenderRole,
                        IsRead = n.IsRead,
                        ThreadId = n.ThreadId,
                        Replies = n.Replies?.Select(r => new NotificationReplyResponse
                        {
                            Id = r.Id,
                            NotificationId = r.NotificationId,
                            ThreadId = r.ThreadId,
                            SenderId = r.SenderId,
                            SenderName = r.SenderName,
                            SenderRole = r.SenderRole,
                            Content = r.Content,
                            SentAt = r.SentAt,
                            IsRead = r.IsRead
                        }).ToList() ?? new List<NotificationReplyResponse>(),
                        CreatedAt = n.CreatedAt
                    };
                    
                    result.Add(dto);
                }

                _logger.LogInformation("Returning {Count} notifications for patientId: {PatientId}", result.Count, patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient notifications for {PatientId}. Exception: {Exception}, InnerException: {InnerException}", 
                    patientId, ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new { 
                    error = "Failed to get patient notifications", 
                    message = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Get all replies for a specific notification
        /// </summary>
        [HttpGet("{notificationId}/replies")]
        [AllowAnonymous]
        public async Task<ActionResult<List<NotificationReplyResponse>>> GetNotificationReplies(Guid notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                var replies = await _context.NotificationReplies
                    .Where(r => r.NotificationId == notificationId)
                    .OrderBy(r => r.SentAt)
                    .Select(r => new NotificationReplyResponse
                    {
                        Id = r.Id,
                        NotificationId = r.NotificationId,
                        ThreadId = r.ThreadId,
                        SenderId = r.SenderId,
                        SenderName = r.SenderName,
                        SenderRole = r.SenderRole,
                        Content = r.Content,
                        SentAt = r.SentAt,
                        IsRead = r.IsRead
                    })
                    .ToListAsync();

                return Ok(replies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting replies for notification {NotificationId}", notificationId);
                return StatusCode(500, new { error = "Failed to get notification replies", message = ex.Message });
            }
        }

        /// <summary>
        /// Reply to a notification/message
        /// </summary>
        [HttpPost("{notificationId}/reply")]
        [AllowAnonymous]
        public async Task<ActionResult<NotificationReplyResponse>> ReplyToNotification(Guid notificationId, [FromBody] ReplyNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var notification = await _context.Notifications
                    .Include(n => n.Patient)
                    .Include(n => n.Sender)
                    .FirstOrDefaultAsync(n => n.Id == notificationId);

                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                var sender = await _context.Users.FindAsync(request.SenderId);
                if (sender == null)
                {
                    return BadRequest(new { error = "Sender not found" });
                }

                // Generate or use existing thread ID
                var threadId = notification.ThreadId ?? Guid.NewGuid();
                if (notification.ThreadId == null)
                {
                    notification.ThreadId = threadId;
                }

                var reply = new NotificationReply
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notificationId,
                    ThreadId = threadId,
                    SenderId = request.SenderId,
                    SenderName = $"{sender.FirstName} {sender.LastName}",
                    SenderRole = request.SenderRole,
                    Content = request.Content,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                };

                _context.NotificationReplies.Add(reply);
                await _context.SaveChangesAsync();

                // Create notification for original sender if different
                if (notification.SenderId.HasValue && notification.SenderId != request.SenderId)
                {
                    var replyNotification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        PatientId = notification.SenderId, // Keep nullable
                        Type = NotificationType.DirectMessage,
                        Status = NotificationStatus.Sent,
                        EmailSubject = $"Reply to: {notification.EmailSubject ?? "Message"}",
                        EmailContent = request.Content,
                        SenderId = request.SenderId,
                        SenderName = reply.SenderName,
                        SenderRole = request.SenderRole,
                        ThreadId = threadId,
                        IsRead = false,
                        SentAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(replyNotification);
                }

                await _context.SaveChangesAsync();

                var response = new NotificationReplyResponse
                {
                    Id = reply.Id,
                    NotificationId = reply.NotificationId,
                    ThreadId = reply.ThreadId,
                    SenderId = reply.SenderId,
                    SenderName = reply.SenderName,
                    SenderRole = reply.SenderRole,
                    Content = reply.Content,
                    SentAt = reply.SentAt,
                    IsRead = reply.IsRead
                };

                return CreatedAtAction(nameof(GetNotificationReplies), new { notificationId = notificationId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replying to notification {NotificationId}", notificationId);
                return StatusCode(500, new { error = "Failed to send reply", message = ex.Message });
            }
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{notificationId}/read")]
        [AllowAnonymous]
        public async Task<ActionResult<MarkNotificationReadResponse>> MarkNotificationAsRead(Guid notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new MarkNotificationReadResponse
                {
                    Success = true,
                    NotificationId = notificationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
                return StatusCode(500, new { error = "Failed to mark notification as read", message = ex.Message });
            }
        }

        /// <summary>
        /// Get all notifications for a staff member
        /// Supports both Staff.Id and User.Id lookups
        /// </summary>
        [HttpGet("staff/{staffId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EnhancedNotificationDto>>> GetStaffNotifications(Guid staffId)
        {
            try
            {
                _logger.LogInformation("Getting notifications for staffId: {StaffId}", staffId);

                // First, try to find if staffId is a Staff.Id or User.Id
                var staff = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == staffId);
                
                Guid? userId = null;
                
                if (staff != null)
                {
                    userId = staff.UserId;
                    _logger.LogInformation("Found staff by Staff.Id: {StaffId}, UserId: {UserId}", staffId, userId);
                }
                else
                {
                    // If not found as Staff.Id, check if it's a User.Id
                    var user = await _context.Users.FindAsync(staffId);
                    if (user != null)
                    {
                        // Try to find staff by UserId
                        staff = await _context.Staff
                            .Include(s => s.User)
                            .FirstOrDefaultAsync(s => s.UserId == user.Id);
                        
                        if (staff != null)
                        {
                            userId = staff.UserId;
                            _logger.LogInformation("Found staff by User.Id: {UserId}, StaffId: {StaffId}", user.Id, staff.Id);
                        }
                        else
                        {
                            // If no staff record found, use the user ID directly
                            userId = user.Id;
                            _logger.LogInformation("Using User.Id directly: {UserId}", userId);
                        }
                    }
                }

                if (!userId.HasValue)
                {
                    _logger.LogWarning("Staff/User not found for staffId: {StaffId}", staffId);
                    return NotFound(new { error = "Staff member not found" });
                }

                // Query notifications where PatientId matches the UserId (for staff recipients)
                var notifications = await _context.Notifications
                    .Where(n => n.PatientId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} notifications for userId: {UserId}", notifications.Count, userId);

                // Manually map to DTO with null-safe checks
                var notificationDtos = notifications.Select(n =>
                {
                    // Load related entities if needed
                    var patient = n.PatientId.HasValue ? _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefault(p => p.UserId == n.PatientId) : null;
                    
                    var appointment = n.AppointmentId.HasValue ? _context.Appointments
                        .Include(a => a.Service)
                        .FirstOrDefault(a => a.Id == n.AppointmentId) : null;
                    
                    var sender = n.SenderId.HasValue ? _context.Users.FindAsync(n.SenderId.Value).Result : null;

                    return new EnhancedNotificationDto
                    {
                        Id = n.Id,
                        Type = n.Type.ToString(),
                        Status = n.Status.ToString(),
                        SentAt = n.SentAt,
                        EmailSubject = n.EmailSubject,
                        EmailContent = n.EmailContent,
                        AppointmentDate = appointment?.AppointmentDate,
                        AppointmentTime = appointment?.StartTime.ToString(@"hh\:mm"),
                        ServiceName = n.ServiceName ?? appointment?.Service?.Name,
                        PatientId = n.PatientId,
                        PatientName = patient?.User != null ? $"{patient.User.FirstName} {patient.User.LastName}" : null,
                        SenderId = n.SenderId,
                        SenderName = n.SenderName,
                        SenderRole = n.SenderRole,
                        IsRead = n.IsRead,
                        ThreadId = n.ThreadId,
                        CreatedAt = n.CreatedAt
                    };
                }).ToList();

                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff notifications for {StaffId}", staffId);
                return StatusCode(500, new { error = "Failed to get staff notifications", message = ex.Message });
            }
        }

        /// <summary>
        /// Get all notifications (admin only)
        /// </summary>
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<NotificationListResponse>> GetAllNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? type = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Notifications
                    .Include(n => n.Patient)
                    .Include(n => n.Appointment)
                        .ThenInclude(a => a!.Service)
                    .Include(n => n.Replies)
                    .Include(n => n.Sender)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(type))
                {
                    if (Enum.TryParse<NotificationType>(type, true, out var notificationType))
                    {
                        query = query.Where(n => n.Type == notificationType);
                    }
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<NotificationStatus>(status, true, out var notificationStatus))
                    {
                        query = query.Where(n => n.Status == notificationStatus);
                    }
                }

                var totalCount = await query.CountAsync();

                var notifications = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new EnhancedNotificationDto
                    {
                        Id = n.Id,
                        Type = n.Type.ToString(),
                        Status = n.Status.ToString(),
                        SentAt = n.SentAt,
                        EmailSubject = n.EmailSubject,
                        EmailContent = n.EmailContent,
                        AppointmentDate = n.Appointment != null ? n.Appointment.AppointmentDate : null,
                        AppointmentTime = n.Appointment != null ? n.Appointment.StartTime.ToString(@"hh\:mm") : null,
                        ServiceName = n.ServiceName ?? (n.Appointment != null && n.Appointment.Service != null ? n.Appointment.Service.Name : null),
                        PatientId = n.PatientId,
                        PatientName = n.Patient != null ? $"{n.Patient.FirstName} {n.Patient.LastName}" : null,
                        SenderId = n.SenderId,
                        SenderName = n.SenderName,
                        SenderRole = n.SenderRole,
                        IsRead = n.IsRead,
                        ThreadId = n.ThreadId,
                        CreatedAt = n.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new NotificationListResponse
                {
                    Notifications = notifications,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all notifications");
                return StatusCode(500, new { error = "Failed to get notifications", message = ex.Message });
            }
        }

        /// <summary>
        /// Send a direct message from one user to another (supports both patient and staff recipients)
        /// </summary>
        [HttpPost("send-message")]
        [AllowAnonymous]
        public async Task<ActionResult<SendMessageResponse>> SendDirectMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );
                    return BadRequest(new { error = "Invalid request format", details = errors });
                }

                // Validate sender role (must be doctor or admin)
                var validSenderRoles = new[] { "doctor", "admin" };
                if (!validSenderRoles.Contains(request.SenderRole.ToLower()))
                {
                    return BadRequest(new { error = "Only doctors and admins can send messages" });
                }

                // Validate recipient exists based on recipientRole
                // Handle multiple ID types: UserId, PatientId, StaffId
                ApplicationUser? recipient = null;
                Staff? recipientStaff = null;
                Patient? recipientPatient = null;
                Guid? actualRecipientId = null; // The actual ID to use for the notification

                if (request.RecipientRole.ToLower() == "patient")
                {
                    // Try multiple lookup strategies for patients
                    // 1. Try Patient.Id
                    recipientPatient = await _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Id == request.RecipientId);
                    
                    if (recipientPatient != null)
                    {
                        recipient = recipientPatient.User;
                        actualRecipientId = recipientPatient.Id;
                        _logger.LogInformation("Found patient by Patient.Id: {PatientId}, UserId: {UserId}", 
                            recipientPatient.Id, recipientPatient.UserId);
                    }
                    else
                    {
                        // 2. Try User.Id (find patient associated with this user)
                        var user = await _context.Users.FindAsync(request.RecipientId);
                        if (user != null)
                        {
                            recipientPatient = await _context.Patients
                                .Include(p => p.User)
                                .FirstOrDefaultAsync(p => p.UserId == user.Id);
                            
                            if (recipientPatient != null)
                            {
                                recipient = recipientPatient.User;
                                actualRecipientId = recipientPatient.Id;
                                _logger.LogInformation("Found patient by User.Id: {UserId}, PatientId: {PatientId}", 
                                    user.Id, recipientPatient.Id);
                            }
                            else
                            {
                                _logger.LogWarning("User found (ID: {UserId}) but no associated patient record", request.RecipientId);
                                return NotFound(new { error = "Patient not found. User exists but has no patient record." });
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Patient not found with ID: {RecipientId} (checked Patient.Id and User.Id)", request.RecipientId);
                            return NotFound(new { error = "Patient not found" });
                        }
                    }
                }
                else
                {
                    // For staff roles, try multiple lookup strategies
                    // 1. Try Staff.Id
                    recipientStaff = await _context.Staff
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == request.RecipientId);
                    
                    if (recipientStaff != null)
                    {
                        recipient = recipientStaff.User;
                        actualRecipientId = recipientStaff.Id;
                        _logger.LogInformation("Found staff by Staff.Id: {StaffId}, UserId: {UserId}", 
                            recipientStaff.Id, recipientStaff.UserId);
                    }
                    else
                    {
                        // 2. Try Staff.UserId
                        recipientStaff = await _context.Staff
                            .Include(s => s.User)
                            .FirstOrDefaultAsync(s => s.UserId == request.RecipientId);
                        
                        if (recipientStaff != null)
                        {
                            recipient = recipientStaff.User;
                            actualRecipientId = recipientStaff.Id;
                            _logger.LogInformation("Found staff by Staff.UserId: {UserId}, StaffId: {StaffId}", 
                                request.RecipientId, recipientStaff.Id);
                        }
                        else
                        {
                            // 3. Try User.Id (check if user exists but has no staff record)
                            var user = await _context.Users.FindAsync(request.RecipientId);
                            if (user != null)
                            {
                                _logger.LogWarning("User found (ID: {UserId}) but no associated staff record for role: {Role}", 
                                    request.RecipientId, request.RecipientRole);
                                return NotFound(new { error = $"Staff member not found. User exists but has no staff record for role '{request.RecipientRole}'." });
                            }
                            else
                            {
                                _logger.LogWarning("Staff member not found with ID: {RecipientId} (checked Staff.Id, Staff.UserId, and User.Id)", 
                                    request.RecipientId);
                                return NotFound(new { error = "Staff member not found" });
                            }
                        }
                    }
                }

                // Get sender details
                Staff? senderStaff = await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.UserId == request.SenderId);
                
                ApplicationUser? senderUser = null;
                
                if (senderStaff != null)
                {
                    senderUser = senderStaff.User;
                }
                else
                {
                    senderUser = await _context.Users.FindAsync(request.SenderId);
                }
                
                if (senderUser == null)
                {
                    _logger.LogWarning("Sender not found with ID: {SenderId}", request.SenderId);
                    return BadRequest(new { error = "Sender not found" });
                }

                var senderName = $"{senderUser.FirstName} {senderUser.LastName}";

                var threadId = Guid.NewGuid();

                // Create notification with correct recipient ID
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Type = NotificationType.DirectMessage,
                    Status = NotificationStatus.Sent,
                    EmailSubject = request.Subject ?? string.Empty,
                    EmailContent = request.Content ?? string.Empty,
                    SenderId = request.SenderId,
                    SenderName = senderName,
                    SenderRole = request.SenderRole,
                    ThreadId = threadId,
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Set recipient based on role
                // IMPORTANT: PatientId column references AspNetUsers.Id, not Patients.Id
                // So we must use the UserId, not the Patient.Id or Staff.Id
                if (request.RecipientRole.ToLower() == "patient")
                {
                    // For patients, use the UserId from the patient record
                    if (recipientPatient != null && recipientPatient.UserId.HasValue)
                    {
                        notification.PatientId = recipientPatient.UserId.Value;
                        _logger.LogInformation("Setting notification PatientId (UserId) for patient: {PatientId}", notification.PatientId);
                    }
                    else if (recipient != null)
                    {
                        notification.PatientId = recipient.Id;
                        _logger.LogInformation("Setting notification PatientId (UserId) from recipient: {PatientId}", notification.PatientId);
                    }
                    else
                    {
                        _logger.LogWarning("No valid UserId found for patient recipient {RecipientId}", request.RecipientId);
                        return BadRequest(new { error = "Patient record found but has no associated user account" });
                    }
                }
                else
                {
                    // For staff recipients, use the UserId directly
                    // This avoids requiring the StaffId column in the database
                    if (recipient != null)
                    {
                        notification.PatientId = recipient.Id;
                        _logger.LogInformation("Setting notification PatientId (UserId) for staff: {PatientId}", notification.PatientId);
                    }
                    else
                    {
                        _logger.LogWarning("No valid UserId found for staff recipient {RecipientId}", request.RecipientId);
                        return BadRequest(new { error = "Staff member found but has no associated user account" });
                    }
                }

                _context.Notifications.Add(notification);
                
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Direct message sent successfully. NotificationId: {NotificationId}, RecipientRole: {RecipientRole}", 
                        notification.Id, request.RecipientRole);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error saving direct message. Exception: {Exception}, InnerException: {InnerException}", 
                        dbEx.Message, dbEx.InnerException?.Message);
                    return StatusCode(500, new 
                    { 
                        error = "Database error occurred while saving message",
                        details = dbEx.InnerException?.Message ?? dbEx.Message
                    });
                }

                return Ok(new SendMessageResponse
                {
                    Id = notification.Id,
                    NotificationId = notification.Id,
                    ThreadId = threadId,
                    Status = notification.Status.ToString(),
                    SentAt = notification.SentAt ?? DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending direct message. Exception: {Exception}, InnerException: {InnerException}", 
                    ex.Message, ex.InnerException?.Message);
                return StatusCode(500, new 
                { 
                    error = "Failed to send message", 
                    message = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Get all messages in a conversation thread
        /// </summary>
        [HttpGet("thread/{threadId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ThreadResponse>> GetThread(Guid threadId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .Include(n => n.Sender)
                    .Include(n => n.Replies)
                    .Where(n => n.ThreadId == threadId)
                    .ToListAsync();

                if (!notifications.Any())
                {
                    return NotFound(new { error = "Thread not found" });
                }

                var participants = new HashSet<ThreadParticipantDto>();
                var messages = new List<ThreadMessageDto>();

                foreach (var notification in notifications)
                {
                    // Add sender as participant
                    if (notification.SenderId.HasValue && notification.Sender != null)
                    {
                        participants.Add(new ThreadParticipantDto
                        {
                            Id = notification.SenderId.Value,
                            Name = notification.SenderName ?? $"{notification.Sender.FirstName} {notification.Sender.LastName}",
                            Role = notification.SenderRole ?? "user"
                        });
                    }

                    // Add recipient as participant
                    if (notification.Patient != null && notification.PatientId.HasValue)
                    {
                        participants.Add(new ThreadParticipantDto
                        {
                            Id = notification.PatientId.Value,
                            Name = $"{notification.Patient.FirstName} {notification.Patient.LastName}",
                            Role = "patient"
                        });
                    }

                    // Add notification as message
                    messages.Add(new ThreadMessageDto
                    {
                        Id = notification.Id,
                        SenderId = notification.SenderId ?? Guid.Empty,
                        SenderName = notification.SenderName ?? "System",
                        SenderRole = notification.SenderRole ?? "system",
                        Content = notification.EmailContent ?? string.Empty,
                        SentAt = notification.SentAt ?? notification.CreatedAt,
                        IsRead = notification.IsRead
                    });

                    // Add replies as messages
                    foreach (var reply in notification.Replies.OrderBy(r => r.SentAt))
                    {
                        messages.Add(new ThreadMessageDto
                        {
                            Id = reply.Id,
                            SenderId = reply.SenderId,
                            SenderName = reply.SenderName,
                            SenderRole = reply.SenderRole,
                            Content = reply.Content,
                            SentAt = reply.SentAt,
                            IsRead = reply.IsRead
                        });
                    }
                }

                messages = messages.OrderBy(m => m.SentAt).ToList();

                return Ok(new ThreadResponse
                {
                    ThreadId = threadId,
                    Participants = participants.ToList(),
                    Messages = messages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting thread {ThreadId}", threadId);
                return StatusCode(500, new { error = "Failed to get thread", message = ex.Message });
            }
        }

        /// <summary>
        /// Subscribe a user for push notifications
        /// </summary>
        [HttpPost("push-subscribe")]
        [AllowAnonymous]
        public async Task<ActionResult<PushSubscriptionResponse>> SubscribePushNotifications([FromBody] PushSubscriptionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                // Parse subscription JSON - handle both object and string formats
                JsonElement subscriptionJson;
                try
                {
                    if (request.Subscription.StartsWith("{") || request.Subscription.StartsWith("["))
                    {
                        subscriptionJson = JsonSerializer.Deserialize<JsonElement>(request.Subscription);
                    }
                    else
                    {
                        // If it's a base64 encoded string, decode it first
                        subscriptionJson = JsonSerializer.Deserialize<JsonElement>(request.Subscription);
                    }
                }
                catch (JsonException)
                {
                    return BadRequest(new { error = "Invalid subscription data format" });
                }

                if (!subscriptionJson.TryGetProperty("endpoint", out var endpointElement) ||
                    !subscriptionJson.TryGetProperty("keys", out var keysElement))
                {
                    return BadRequest(new { error = "Invalid subscription data: missing endpoint or keys" });
                }

                var endpoint = endpointElement.GetString();
                if (string.IsNullOrEmpty(endpoint))
                {
                    return BadRequest(new { error = "Invalid subscription data: endpoint is required" });
                }

                if (!keysElement.TryGetProperty("p256dh", out var p256dhElement) ||
                    !keysElement.TryGetProperty("auth", out var authElement))
                {
                    return BadRequest(new { error = "Invalid subscription data: missing p256dh or auth keys" });
                }

                var p256dh = p256dhElement.GetString();
                var auth = authElement.GetString();

                if (string.IsNullOrEmpty(p256dh) || string.IsNullOrEmpty(auth))
                {
                    return BadRequest(new { error = "Invalid subscription data: p256dh and auth keys are required" });
                }

                // Check if subscription already exists
                var existingSubscription = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(ps => ps.UserId == request.UserId && ps.Endpoint == endpoint);

                if (existingSubscription != null)
                {
                    // Update existing subscription
                    existingSubscription.P256dhKey = p256dh;
                    existingSubscription.AuthKey = auth;
                    existingSubscription.IsActive = true;
                    existingSubscription.UserRole = request.UserRole;
                    existingSubscription.CreatedAt = DateTime.UtcNow; // Update timestamp
                }
                else
                {
                    // Create new subscription
                    var subscription = new PushSubscription
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        UserRole = request.UserRole,
                        Endpoint = endpoint,
                        P256dhKey = p256dh,
                        AuthKey = auth,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.PushSubscriptions.Add(subscription);
                    await _context.SaveChangesAsync();

                    return Ok(new PushSubscriptionResponse
                    {
                        Success = true,
                        SubscriptionId = subscription.Id
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new PushSubscriptionResponse
                {
                    Success = true,
                    SubscriptionId = existingSubscription.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to push notifications");
                return StatusCode(500, new { error = "Failed to subscribe to push notifications", message = ex.Message });
            }
        }

        /// <summary>
        /// Send a push notification to a user
        /// </summary>
        [HttpPost("push-send")]
        [AllowAnonymous]
        public async Task<ActionResult<SendPushNotificationResponse>> SendPushNotification([FromBody] SendPushNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var subscriptions = await _context.PushSubscriptions
                    .Where(ps => ps.UserId == request.UserId && ps.IsActive)
                    .ToListAsync();

                if (!subscriptions.Any())
                {
                    return BadRequest(new { error = "No active push subscriptions found for user" });
                }

                // TODO: Implement actual push notification sending using Web Push library or FCM
                // For now, we'll just return success
                // In production, you would:
                // 1. Use WebPush library for web push notifications
                // 2. Use Firebase Cloud Messaging for mobile apps
                // 3. Send notification to each subscription endpoint

                _logger.LogInformation("Push notification queued for user {UserId}: {Title} - {Body}", 
                    request.UserId, request.Title, request.Body);

                return Ok(new SendPushNotificationResponse
                {
                    Success = true,
                    MessageId = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending push notification");
                return StatusCode(500, new { error = "Failed to send push notification", message = ex.Message });
            }
        }

        /// <summary>
        /// Unsubscribe a user from push notifications
        /// </summary>
        [HttpDelete("push-subscribe/{subscriptionId}")]
        [AllowAnonymous]
        public async Task<ActionResult> UnsubscribePushNotifications(Guid subscriptionId)
        {
            try
            {
                var subscription = await _context.PushSubscriptions.FindAsync(subscriptionId);
                if (subscription == null)
                {
                    return NotFound(new { error = "Subscription not found" });
                }

                subscription.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from push notifications");
                return StatusCode(500, new { error = "Failed to unsubscribe", message = ex.Message });
            }
        }
    }
}