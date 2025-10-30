using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HopewellClinicApi.Services
{
    /// <summary>
    /// Service for handling email notifications and scheduled reminders
    /// </summary>
    public interface INotificationService
    {
        Task<NotificationResponse> SendCustomNotificationAsync(SendCustomEmailRequest request);
        Task<BulkNotificationResponse> SendBulkNotificationAsync(SendBulkEmailRequest request);
        Task<List<NotificationHistoryItem>> GetScheduledNotificationsAsync();
        Task<PatientNotificationHistoryResponse> GetNotificationHistoryAsync(NotificationHistoryRequest request);
        Task<PatientNotificationHistoryResponse> GetPatientNotificationHistoryAsync(Guid patientId);
        Task<NotificationSettingsResponse> GetNotificationSettingsAsync();
        Task<bool> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request);
        Task<EmailPreviewResponse> PreviewEmailAsync(PreviewEmailRequest request);
        Task<EmailConfigurationTestResponse> TestEmailConfigurationAsync();
        Task ProcessScheduledNotificationsAsync();
        Task Send24HourRemindersAsync();
        Task Send2HourRemindersAsync();
    }

    public class NotificationService : INotificationService
    {
        private readonly HopewellDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            HopewellDbContext context,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Enhanced email sending with support for multiple providers
        /// </summary>
        private async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var provider = emailSettings["Provider"] ?? "SMTP";

                return provider.ToUpper() switch
                {
                    "SMTP" => await SendEmailViaSMTPAsync(to, subject, body, isHtml),
                    "SENDGRID" => await SendEmailViaSendGridAsync(to, subject, body, isHtml),
                    "AWS_SES" => await SendEmailViaAWSSESAsync(to, subject, body, isHtml),
                    "GMAIL" => await SendEmailViaGmailAsync(to, subject, body, isHtml),
                    _ => await SendEmailViaSMTPAsync(to, subject, body, isHtml)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                return false;
            }
        }

        private async Task<bool> SendEmailViaSMTPAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpHost = emailSettings["SmtpHost"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var smtpUser = emailSettings["SmtpUser"];
                var smtpPass = emailSettings["SmtpPass"];
                var fromEmail = emailSettings["FromEmail"] ?? "noreply@hopewellclinic.com";

                _logger.LogInformation("SMTP Configuration - Host: {Host}, Port: {Port}, User: {User}, From: {From}", 
                    smtpHost, smtpPort, smtpUser, fromEmail);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Hopewell Clinic", fromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                    bodyBuilder.HtmlBody = body;
                else
                    bodyBuilder.TextBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                _logger.LogInformation("Attempting SMTP connection to {Host}:{Port}", smtpHost, smtpPort);
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("SMTP connection established, authenticating...");
                await client.AuthenticateAsync(smtpUser, smtpPass);
                _logger.LogInformation("SMTP authentication successful, sending email...");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully via SMTP to {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP email sending failed to {To}", to);
                return false;
            }
        }

        private async Task<bool> SendEmailViaSendGridAsync(string to, string subject, string body, bool isHtml)
        {
            // TODO: Implement SendGrid integration
            _logger.LogWarning("SendGrid integration not implemented yet");
            return false;
        }

        private async Task<bool> SendEmailViaAWSSESAsync(string to, string subject, string body, bool isHtml)
        {
            // TODO: Implement AWS SES integration
            _logger.LogWarning("AWS SES integration not implemented yet");
            return false;
        }

        private async Task<bool> SendEmailViaGmailAsync(string to, string subject, string body, bool isHtml)
        {
            // TODO: Implement Gmail integration
            _logger.LogWarning("Gmail integration not implemented yet");
            return false;
        }

        public async Task<NotificationResponse> SendCustomNotificationAsync(SendCustomEmailRequest request)
        {
            try
            {
                _logger.LogInformation("Looking up patient/user with ID: {PatientId}", request.PatientId);

                // Multi-ID lookup strategy: Try Patient.Id, then User.Id
                Patient? patient = null;
                Guid? userId = null;
                Guid? actualPatientId = null; // The Patient.Id to use for the notification
                string lookupType = "Patient ID";

                // Strategy 1: Try Patient.Id
                patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId);

                if (patient != null)
                {
                    userId = patient.UserId;
                    actualPatientId = patient.Id;
                    _logger.LogInformation("Found patient by Patient.Id: {PatientId}, UserId: {UserId}", 
                        patient.Id, patient.UserId);
                }
                else
                {
                    // Strategy 2: Try User.Id (find patient associated with this user)
                    _logger.LogInformation("Patient not found with Patient.Id: {PatientId}, trying User.Id lookup", request.PatientId);
                    
                    var user = await _context.Users.FindAsync(request.PatientId);
                    if (user != null)
                    {
                        _logger.LogInformation("Found user with User.Id: {UserId}, searching for associated patient", request.PatientId);
                        userId = user.Id;
                        
                        // Find patient associated with this user
                        patient = await _context.Patients
                            .Include(p => p.User)
                            .FirstOrDefaultAsync(p => p.UserId == userId);

                        if (patient != null)
                        {
                            actualPatientId = patient.Id;
                            lookupType = "User ID";
                            _logger.LogInformation("Found patient by User.Id: {UserId}, Patient.Id: {PatientId}",
                                userId, patient.Id);
                        }
                        else
                        {
                            _logger.LogWarning("User found with User.Id: {UserId}, but no associated patient record exists", userId);
                            return new NotificationResponse
                            {
                                Success = false,
                                Error = $"User found (ID: {request.PatientId}), but no associated patient record exists. Cannot send notification to non-patient users."
                            };
                        }
                    }
                }

                if (patient == null)
                {
                    _logger.LogWarning("No patient or user found with ID: {PatientId} (checked Patient.Id and User.Id)", request.PatientId);
                    return new NotificationResponse
                    {
                        Success = false,
                        Error = $"No patient or user found with ID '{request.PatientId}'. Searched as both Patient ID and User ID."
                    };
                }

                if (patient.UserId == null)
                {
                    _logger.LogWarning("Patient {PatientId} (found via {LookupType}) has no associated UserId", 
                        patient.Id, lookupType ?? "Unknown");
                    return new NotificationResponse
                    {
                        Success = false,
                        Error = $"Patient found (ID: {patient.Id}, looked up via {lookupType ?? "database lookup"}), but has no associated user account (UserId is null)"
                    };
                }

                // Verify User is loaded and not null
                if (patient.User == null)
                {
                    _logger.LogWarning("Patient {PatientId} User navigation property is null, attempting to load UserId: {UserId}", 
                        patient.Id, patient.UserId);
                    await _context.Entry(patient).Reference(p => p.User).LoadAsync();
                    
                    if (patient.User == null)
                    {
                        _logger.LogError("Patient {PatientId} User could not be loaded. UserId exists ({UserId}) but User record not found in database", 
                            patient.Id, patient.UserId);
                        return new NotificationResponse
                        {
                            Success = false,
                            Error = $"Patient found (ID: {patient.Id}), but associated user account (UserId: {patient.UserId}) could not be loaded from database. User may have been deleted."
                        };
                    }
                }
                
                _logger.LogInformation("Successfully loaded patient and user. Patient ID: {PatientId}, User ID: {UserId}, Email: {Email}", 
                    patient.Id, patient.UserId, patient.User?.Email ?? "null");

                // Verify email exists
                if (string.IsNullOrWhiteSpace(patient.User.Email))
                {
                    _logger.LogWarning("Patient {PatientId} has no email address", request.PatientId);
                    return new NotificationResponse
                    {
                        Success = false,
                        Error = "Patient has no email address configured"
                    };
                }

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    PatientId = patient.UserId.Value, // Store UserId (Notifications.PatientId references AspNetUsers.Id)
                    Type = NotificationType.Custom,
                    Status = NotificationStatus.Scheduled,
                    EmailSubject = request.Subject ?? string.Empty,
                    EmailContent = request.Message ?? string.Empty,
                    ThreadId = Guid.NewGuid(), // Generate thread ID for replies
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _logger.LogInformation("Created notification with PatientId (UserId): {UserId}, resolved from {LookupType}: {OriginalId}", 
                    patient.UserId.Value, lookupType, request.PatientId);

                _context.Notifications.Add(notification);
                
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Notification {NotificationId} created successfully for patient {PatientId}", notification.Id, request.PatientId);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error saving notification for patient {PatientId}. Exception: {Exception}, InnerException: {InnerException}", 
                        request.PatientId, dbEx.Message, dbEx.InnerException?.Message);
                    return new NotificationResponse
                    {
                        Success = false,
                        Error = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}"
                    };
                }

                // Attempt to send email (don't fail notification creation if email fails)
                bool emailSent = false;
                try
                {
                    _logger.LogInformation("Attempting to send email to {Email} for patient {PatientId}", patient.User.Email, request.PatientId);
                    emailSent = await SendEmailAsync(patient.User.Email, request.Subject ?? string.Empty, request.Message ?? string.Empty, true);
                    _logger.LogInformation("Email sending result: {Result}", emailSent);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email to {Email} for patient {PatientId}. Exception: {Exception}", 
                        patient.User.Email, request.PatientId, emailEx.Message);
                    emailSent = false;
                }

                // Update notification status based on email result
                try
                {
                    if (emailSent)
                    {
                        notification.Status = NotificationStatus.Sent;
                        notification.SentAt = DateTime.UtcNow;
                    }
                    else
                    {
                        notification.Status = NotificationStatus.Failed;
                        notification.ErrorMessage = "Email sending failed";
                    }

                    notification.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Error updating notification status. Exception: {Exception}", updateEx.Message);
                    // Don't fail the response if status update fails - notification was already created
                }

                return new NotificationResponse
                {
                    Success = true, // Always return success if notification was created
                    MessageId = emailSent ? "sent" : null,
                    NotificationId = notification.Id.ToString(),
                    Message = emailSent ? "Email sent successfully" : "Notification created but email sending failed",
                    Error = emailSent ? null : "Email sending failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom notification");
                return new NotificationResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<BulkNotificationResponse> SendBulkNotificationAsync(SendBulkEmailRequest request)
        {
            try
            {
                var results = new List<NotificationResponse>();
                int totalSent = 0;
                int totalFailed = 0;

                foreach (var patientId in request.PatientIds)
                {
                    var customRequest = new SendCustomEmailRequest
                    {
                        PatientId = patientId,
                        Subject = request.Subject,
                        Message = request.Message
                    };

                    var result = await SendCustomNotificationAsync(customRequest);
                    results.Add(result);

                    if (result.Success)
                        totalSent++;
                    else
                        totalFailed++;
                }

                var successRate = request.PatientIds.Count > 0 ? (double)totalSent / request.PatientIds.Count * 100 : 0;

                return new BulkNotificationResponse
                {
                    Success = totalFailed == 0,
                    Results = results,
                    TotalSent = totalSent,
                    TotalFailed = totalFailed,
                    SuccessRate = successRate,
                    Message = $"Bulk notification completed. {totalSent} sent, {totalFailed} failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk notification");
                return new BulkNotificationResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<List<NotificationHistoryItem>> GetScheduledNotificationsAsync()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .Where(n => n.Status == NotificationStatus.Scheduled)
                    .OrderBy(n => n.ScheduledFor)
                    .ToListAsync();

                return notifications.Select(n => new NotificationHistoryItem
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
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled notifications");
                return new List<NotificationHistoryItem>();
            }
        }

        public async Task<PatientNotificationHistoryResponse> GetNotificationHistoryAsync(NotificationHistoryRequest request)
        {
            try
            {
                var query = _context.Notifications
                    .Include(n => n.Patient)
                    .AsQueryable();

                if (request.PatientId.HasValue)
                    query = query.Where(n => n.PatientId == request.PatientId.Value);

                if (!string.IsNullOrEmpty(request.Type))
                    query = query.Where(n => n.Type.ToString() == request.Type);

                if (!string.IsNullOrEmpty(request.Status))
                    query = query.Where(n => n.Status.ToString() == request.Status);

                if (request.StartDate.HasValue)
                    query = query.Where(n => n.CreatedAt >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(n => n.CreatedAt <= request.EndDate.Value);

                if (request.HasAppointment.HasValue)
                {
                    if (request.HasAppointment.Value)
                        query = query.Where(n => n.AppointmentId != null);
                    else
                        query = query.Where(n => n.AppointmentId == null);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                var notifications = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var items = notifications.Select(n => new NotificationHistoryItem
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
                }).ToList();

                return new PatientNotificationHistoryResponse
                {
                    Notifications = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification history");
                return new PatientNotificationHistoryResponse
                {
                    Notifications = new List<NotificationHistoryItem>(),
                    TotalCount = 0,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = 0
                };
            }
        }

        public async Task<PatientNotificationHistoryResponse> GetPatientNotificationHistoryAsync(Guid patientId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .Where(n => n.PatientId == patientId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                var items = notifications.Select(n => new NotificationHistoryItem
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
                }).ToList();

                return new PatientNotificationHistoryResponse
                {
                    Notifications = items,
                    TotalCount = items.Count,
                    Page = 1,
                    PageSize = items.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient notification history for {PatientId}", patientId);
                return new PatientNotificationHistoryResponse
                {
                    Notifications = new List<NotificationHistoryItem>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 0,
                    TotalPages = 0
                };
            }
        }

        public async Task<NotificationSettingsResponse> GetNotificationSettingsAsync()
        {
            try
            {
                var settings = await _context.NotificationSettings.FirstOrDefaultAsync();
                if (settings == null)
                {
                    // Create default settings
                    settings = new NotificationSettings
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.NotificationSettings.Add(settings);
                    await _context.SaveChangesAsync();
                }

                return new NotificationSettingsResponse
                {
                    Id = settings.Id,
                    AutoReminder24h = settings.AutoReminder24h,
                    AutoReminder2h = settings.AutoReminder2h,
                    EmailNotifications = settings.EmailNotifications,
                    SmsNotifications = settings.SmsNotifications,
                    AppointmentConfirmations = settings.AppointmentConfirmations,
                    PrescriptionAlerts = settings.PrescriptionAlerts,
                    TestResultAlerts = settings.TestResultAlerts,
                    InsuranceReminders = settings.InsuranceReminders,
                    ClinicEmail = settings.ClinicEmail,
                    ClinicPhone = settings.ClinicPhone,
                    ClinicAddress = settings.ClinicAddress,
                    UpdatedAt = settings.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification settings");
                return new NotificationSettingsResponse
                {
                    Id = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<bool> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request)
        {
            try
            {
                var settings = await _context.NotificationSettings.FirstOrDefaultAsync();
                if (settings == null)
                {
                    settings = new NotificationSettings
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.NotificationSettings.Add(settings);
                }

                // Update reminder settings if provided
                if (request.Reminder24hEnabled.HasValue)
                {
                    settings.AutoReminder24h = request.Reminder24hEnabled.Value;
                }

                if (request.Reminder2hEnabled.HasValue)
                {
                    settings.AutoReminder2h = request.Reminder2hEnabled.Value;
                }

                // Update other notification settings if provided
                if (request.EmailNotifications.HasValue)
                {
                    settings.EmailNotifications = request.EmailNotifications.Value;
                }

                if (request.SmsNotifications.HasValue)
                {
                    settings.SmsNotifications = request.SmsNotifications.Value;
                }

                if (request.AppointmentConfirmations.HasValue)
                {
                    settings.AppointmentConfirmations = request.AppointmentConfirmations.Value;
                }

                if (request.PrescriptionAlerts.HasValue)
                {
                    settings.PrescriptionAlerts = request.PrescriptionAlerts.Value;
                }

                if (request.TestResultAlerts.HasValue)
                {
                    settings.TestResultAlerts = request.TestResultAlerts.Value;
                }

                if (request.InsuranceReminders.HasValue)
                {
                    settings.InsuranceReminders = request.InsuranceReminders.Value;
                }

                // Update clinic contact information if provided
                if (request.ClinicEmail != null)
                {
                    settings.ClinicEmail = request.ClinicEmail;
                }

                if (request.ClinicPhone != null)
                {
                    settings.ClinicPhone = request.ClinicPhone;
                }

                if (request.ClinicAddress != null)
                {
                    settings.ClinicAddress = request.ClinicAddress;
                }

                settings.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings");
                return false;
            }
        }

        public async Task<EmailPreviewResponse> PreviewEmailAsync(PreviewEmailRequest request)
        {
            try
            {
                // Simple preview - just return the subject and message as HTML
                var htmlContent = $"<html><body><h2>{request.Subject}</h2><p>{request.Message.Replace("\n", "<br>")}</p></body></html>";
                var textContent = request.Message;

                return new EmailPreviewResponse
                {
                    Subject = request.Subject,
                    HtmlContent = htmlContent,
                    TextContent = textContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing email");
                return new EmailPreviewResponse
                {
                    Subject = request.Subject,
                    HtmlContent = "<p>Error generating preview</p>",
                    TextContent = "Error generating preview"
                };
            }
        }

        public async Task<EmailConfigurationTestResponse> TestEmailConfigurationAsync()
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var testEmail = emailSettings["FromEmail"]; // Send test email to itself
                var subject = "Hopewell Clinic - Test Email Configuration";
                var body = "This is a test email from Hopewell Clinic notification system. If you received this, your email configuration is working!";

                bool sent = await SendEmailAsync(testEmail, subject, body, true);

                return new EmailConfigurationTestResponse
                {
                    Success = sent,
                    Message = sent ? "Test email sent successfully!" : "Failed to send test email. Check logs for details."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing email configuration.");
                return new EmailConfigurationTestResponse
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task ProcessScheduledNotificationsAsync()
        {
            try
            {
                var scheduledNotifications = await _context.Notifications
                    .Include(n => n.Patient)
                    .Where(n => n.Status == NotificationStatus.Scheduled && 
                               n.ScheduledFor.HasValue && 
                               n.ScheduledFor <= DateTime.UtcNow)
                    .ToListAsync();

                foreach (var notification in scheduledNotifications)
                {
                    notification.Status = NotificationStatus.Processing;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                foreach (var notification in scheduledNotifications)
                {
                    try
                    {
                        var emailSent = await SendEmailAsync(
                            notification.Patient.Email!, 
                            notification.EmailSubject, 
                            notification.EmailContent, 
                            true);

                        if (emailSent)
                        {
                            notification.Status = NotificationStatus.Sent;
                            notification.SentAt = DateTime.UtcNow;
                        }
                        else
                        {
                            notification.Status = NotificationStatus.Failed;
                            notification.ErrorMessage = "Email sending failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        notification.Status = NotificationStatus.Failed;
                        notification.ErrorMessage = ex.Message;
                        _logger.LogError(ex, "Error processing scheduled notification {NotificationId}", notification.Id);
                    }

                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled notifications");
            }
        }

        public async Task Send24HourRemindersAsync()
        {
            try
            {
                var settings = await GetNotificationSettingsAsync();
                if (!settings.AutoReminder24h)
                    return;

                var tomorrow = DateTime.UtcNow.AddDays(1).Date;
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Service)
                    .Include(a => a.Staff)
                    .Where(a => a.AppointmentDate.Date == tomorrow && 
                               a.Status == "confirmed")
                    .ToListAsync();

                foreach (var appointment in appointments)
                {
                    var subject = "Appointment Reminder - Tomorrow";
                    var message = $"Dear {appointment.Patient.User?.FirstName},\n\n" +
                                $"This is a reminder that you have an appointment tomorrow at {appointment.StartTime} " +
                                $"for {appointment.Service?.Name ?? "your appointment"}.\n\n" +
                                $"Please arrive 15 minutes early.\n\n" +
                                $"Best regards,\nHopewell Clinic";

                    if (appointment.Patient.UserId.HasValue)
                    {
                        var notification = new Notification
                        {
                            Id = Guid.NewGuid(),
                            AppointmentId = appointment.Id,
                            PatientId = appointment.Patient.UserId.Value,
                            Type = NotificationType.Reminder24h,
                            Status = NotificationStatus.Scheduled,
                            ScheduledFor = DateTime.UtcNow.AddMinutes(1), // Send immediately for testing
                            EmailSubject = subject,
                            EmailContent = message,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.Notifications.Add(notification);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending 24-hour reminders");
            }
        }

        public async Task Send2HourRemindersAsync()
        {
            try
            {
                var settings = await GetNotificationSettingsAsync();
                if (!settings.AutoReminder2h)
                    return;

                var twoHoursFromNow = DateTime.UtcNow.AddHours(2);
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Service)
                    .Include(a => a.Staff)
                    .Where(a => a.AppointmentDate.Date == twoHoursFromNow.Date &&
                               a.StartTime <= TimeOnly.FromTimeSpan(twoHoursFromNow.TimeOfDay) &&
                               a.Status == "confirmed")
                    .ToListAsync();

                foreach (var appointment in appointments)
                {
                    var subject = "Appointment Reminder - 2 Hours";
                    var message = $"Dear {appointment.Patient.User?.FirstName},\n\n" +
                                $"This is a reminder that you have an appointment in 2 hours at {appointment.StartTime} " +
                                $"for {appointment.Service?.Name ?? "your appointment"}.\n\n" +
                                $"Please arrive 15 minutes early.\n\n" +
                                $"Best regards,\nHopewell Clinic";

                    if (appointment.Patient.UserId.HasValue)
                    {
                        var notification = new Notification
                        {
                            Id = Guid.NewGuid(),
                            AppointmentId = appointment.Id,
                            PatientId = appointment.Patient.UserId.Value,
                            Type = NotificationType.Reminder2h,
                            Status = NotificationStatus.Scheduled,
                            ScheduledFor = DateTime.UtcNow.AddMinutes(1), // Send immediately for testing
                            EmailSubject = subject,
                            EmailContent = message,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.Notifications.Add(notification);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending 2-hour reminders");
            }
        }
    }
}