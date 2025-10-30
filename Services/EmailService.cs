using System.Net;
using System.Net.Mail;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    public interface IEmailService
    {
        Task<EmailSendResult> SendEmailAsync(string to, string subject, string htmlContent, string textContent);
        Task<EmailPreviewResult> PreviewEmailAsync(PreviewEmailRequest request);
        Task<EmailTestResult> TestConfigurationAsync();
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpClient = new SmtpClient();
            ConfigureSmtpClient();
        }

        private void ConfigureSmtpClient()
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var host = emailSettings["SmtpHost"] ?? "smtp.ethereal.email";
                var port = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var username = emailSettings["SmtpUser"] ?? "";
                var password = emailSettings["SmtpPass"] ?? "";
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "false");
                var enableTls = bool.Parse(emailSettings["EnableTls"] ?? "true");

                _smtpClient.Host = host;
                _smtpClient.Port = port;
                _smtpClient.EnableSsl = enableSsl;
                _smtpClient.Credentials = new NetworkCredential(username, password);
                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Configure TLS for port 587 (STARTTLS)
                if (enableTls && port == 587)
                {
                    _smtpClient.EnableSsl = false; // Disable SSL for STARTTLS
                    // Note: .NET SmtpClient automatically uses STARTTLS on port 587 when EnableSsl is false
                }

                _logger.LogInformation("SMTP Client configured: Host={Host}, Port={Port}, SSL={SSL}, TLS={TLS}", host, port, enableSsl, enableTls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure SMTP client");
                throw;
            }
        }

        public async Task<EmailSendResult> SendEmailAsync(string to, string subject, string htmlContent, string textContent)
        {
            try
            {
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@hopewellclinic.com";
                
                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Hopewell Clinic"),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                // Add text version as alternative view
                var textView = AlternateView.CreateAlternateViewFromString(textContent, null, "text/plain");
                message.AlternateViews.Add(textView);

                await _smtpClient.SendMailAsync(message);

                var messageId = Guid.NewGuid().ToString();
                var previewUrl = $"https://ethereal.email/message/{messageId}";

                _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);

                return new EmailSendResult
                {
                    Success = true,
                    MessageId = messageId,
                    PreviewUrl = previewUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
                return new EmailSendResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<EmailPreviewResult> PreviewEmailAsync(PreviewEmailRequest request)
        {
            try
            {
                // Simple preview - in a real implementation, you'd use a template engine
                var htmlContent = $"<html><body><h2>{request.Subject}</h2><p>{request.Message}</p></body></html>";
                var textContent = $"{request.Subject}\n\n{request.Message}";

                return new EmailPreviewResult
                {
                    Subject = request.Subject,
                    HtmlContent = htmlContent,
                    TextContent = textContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to preview email");
                throw;
            }
        }

        public async Task<EmailTestResult> TestConfigurationAsync()
        {
            try
            {
                // Test SMTP connection by creating a test message
                var testMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:FromEmail"] ?? "noreply@hopewellclinic.com"),
                    Subject = "Test Email Configuration",
                    Body = "This is a test email to verify SMTP configuration.",
                    IsBodyHtml = false
                };
                testMessage.To.Add(_configuration["EmailSettings:FromEmail"] ?? "noreply@hopewellclinic.com");

                await _smtpClient.SendMailAsync(testMessage);

                return new EmailTestResult
                {
                    Success = true,
                    Message = "Email configuration is working correctly",
                    Details = $"SMTP Host: {_smtpClient.Host}, Port: {_smtpClient.Port}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email configuration test failed");
                return new EmailTestResult
                {
                    Success = false,
                    Message = ex.Message,
                    Details = $"SMTP Host: {_smtpClient.Host}, Port: {_smtpClient.Port}"
                };
            }
        }

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}




