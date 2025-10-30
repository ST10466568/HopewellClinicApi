using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    /// <summary>
    /// Background service for processing scheduled notifications and reminders
    /// </summary>
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public NotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessScheduledNotifications(stoppingToken);
                    await Process24HourReminders(stoppingToken);
                    await Process2HourReminders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Notification Background Service");
                }

                await Task.Delay(_period, stoppingToken);
            }

            _logger.LogInformation("Notification Background Service stopped");
        }

        /// <summary>
        /// Process scheduled notifications that are due
        /// </summary>
        private async Task ProcessScheduledNotifications(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var dueNotifications = await context.Notifications
                    .Where(n => n.Status == NotificationStatus.Scheduled && 
                               n.ScheduledFor <= DateTime.UtcNow)
                    .Take(10) // Process in batches
                    .ToListAsync(cancellationToken);

                if (dueNotifications.Any())
                {
                    _logger.LogInformation("Processing {Count} scheduled notifications", dueNotifications.Count);

                    foreach (var notification in dueNotifications)
                    {
                        try
                        {
                            // Update status to processing
                            notification.Status = NotificationStatus.Processing;
                            notification.UpdatedAt = DateTime.UtcNow;
                            await context.SaveChangesAsync(cancellationToken);

                            // Send the notification
                            var success = await SendNotificationEmail(notification, context, cancellationToken);

                            // Update status based on result
                            notification.Status = success ? NotificationStatus.Sent : NotificationStatus.Failed;
                            notification.SentAt = success ? DateTime.UtcNow : null;
                            notification.UpdatedAt = DateTime.UtcNow;

                            if (!success)
                            {
                                notification.ErrorMessage = "Failed to send email";
                            }

                            await context.SaveChangesAsync(cancellationToken);

                            _logger.LogInformation("Notification {Id} processed with status: {Status}", 
                                notification.Id, notification.Status);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing notification {Id}", notification.Id);
                            
                            notification.Status = NotificationStatus.Failed;
                            notification.ErrorMessage = ex.Message;
                            notification.UpdatedAt = DateTime.UtcNow;
                            await context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessScheduledNotifications");
            }
        }

        /// <summary>
        /// Process 24-hour appointment reminders
        /// </summary>
        private async Task Process24HourReminders(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();

            try
            {
                var tomorrow = DateTime.UtcNow.AddDays(1);
                var tomorrowStart = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
                var tomorrowEnd = tomorrowStart.AddDays(1);

                var appointments = await context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Where(a => a.AppointmentDate >= tomorrowStart && 
                               a.AppointmentDate < tomorrowEnd &&
                               a.Status == "confirmed")
                    .ToListAsync(cancellationToken);

                if (appointments.Any())
                {
                    _logger.LogInformation("Processing {Count} 24-hour reminders", appointments.Count);

                    foreach (var appointment in appointments)
                    {
                        try
                        {
                            // Check if reminder already sent
                            var existingReminder = await context.Notifications
                                .FirstOrDefaultAsync(n => n.AppointmentId == appointment.Id && 
                                                         n.Type == NotificationType.Reminder24h &&
                                                         n.Status == NotificationStatus.Sent, 
                                                         cancellationToken);

                            if (existingReminder == null)
                            {
                                await CreateReminderNotification(appointment, NotificationType.Reminder24h, context, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating 24-hour reminder for appointment {Id}", appointment.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Process24HourReminders");
            }
        }

        /// <summary>
        /// Process 2-hour appointment reminders
        /// </summary>
        private async Task Process2HourReminders(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();

            try
            {
                var twoHoursFromNow = DateTime.UtcNow.AddHours(2);
                var twoHoursStart = new DateTime(twoHoursFromNow.Year, twoHoursFromNow.Month, twoHoursFromNow.Day, 
                                                twoHoursFromNow.Hour, 0, 0);
                var twoHoursEnd = twoHoursStart.AddHours(1);

                var appointments = await context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Where(a => a.AppointmentDate >= twoHoursStart && 
                               a.AppointmentDate < twoHoursEnd &&
                               a.Status == "confirmed")
                    .ToListAsync(cancellationToken);

                if (appointments.Any())
                {
                    _logger.LogInformation("Processing {Count} 2-hour reminders", appointments.Count);

                    foreach (var appointment in appointments)
                    {
                        try
                        {
                            // Check if reminder already sent
                            var existingReminder = await context.Notifications
                                .FirstOrDefaultAsync(n => n.AppointmentId == appointment.Id && 
                                                         n.Type == NotificationType.Reminder2h &&
                                                         n.Status == NotificationStatus.Sent, 
                                                         cancellationToken);

                            if (existingReminder == null)
                            {
                                await CreateReminderNotification(appointment, NotificationType.Reminder2h, context, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating 2-hour reminder for appointment {Id}", appointment.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Process2HourReminders");
            }
        }

        /// <summary>
        /// Send notification email
        /// </summary>
        private async Task<bool> SendNotificationEmail(Notification notification, HopewellDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                // Get patient email
                var appointment = await context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(a => a.Id == notification.AppointmentId, cancellationToken);

                if (appointment?.Patient?.User?.Email == null)
                {
                    _logger.LogWarning("No email found for notification {Id}", notification.Id);
                    return false;
                }

                // TODO: Implement actual email sending using NotificationService
                // For now, simulate email sending
                _logger.LogInformation("Simulating email send to {Email} for notification {Id}", 
                    appointment.Patient.User.Email, notification.Id);

                // Simulate processing time
                await Task.Delay(100, cancellationToken);

                return true; // Simulate success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification email for notification {Id}", notification.Id);
                return false;
            }
        }

        /// <summary>
        /// Create reminder notification
        /// </summary>
        private async Task CreateReminderNotification(Appointment appointment, NotificationType type, 
            HopewellDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    PatientId = appointment.Patient.UserId ?? Guid.Empty,
                    Type = type,
                    Status = NotificationStatus.Scheduled,
                    ScheduledFor = DateTime.UtcNow.AddMinutes(1), // Send in 1 minute
                    EmailSubject = $"Appointment Reminder - {appointment.AppointmentDate:MMM dd, yyyy}",
                    EmailContent = GenerateReminderEmailBody(appointment, type),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Notifications.Add(notification);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created {Type} reminder notification for appointment {Id}", 
                    type, appointment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reminder notification for appointment {Id}", appointment.Id);
            }
        }

        /// <summary>
        /// Generate reminder email body
        /// </summary>
        private string GenerateReminderEmailBody(Appointment appointment, NotificationType type)
        {
            var timeUntil = type == NotificationType.Reminder24h ? "24 hours" : "2 hours";
            
            return $@"
                <html>
                <body>
                    <h2>Appointment Reminder</h2>
                    <p>Dear {appointment.Patient.User?.FirstName} {appointment.Patient.User?.LastName},</p>
                    <p>This is a reminder that you have an appointment scheduled in {timeUntil}:</p>
                    <ul>
                        <li><strong>Date:</strong> {appointment.AppointmentDate:MMMM dd, yyyy}</li>
                        <li><strong>Time:</strong> {appointment.StartTime:HH:mm} - {appointment.EndTime:HH:mm}</li>
                        <li><strong>Notes:</strong> {appointment.Notes ?? "None"}</li>
                    </ul>
                    <p>Please arrive 15 minutes early for your appointment.</p>
                    <p>If you need to reschedule or cancel, please contact us as soon as possible.</p>
                    <p>Best regards,<br>Hopewell Community Clinic</p>
                </body>
                </html>";
        }
    }
}
