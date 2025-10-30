using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Services;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailSettingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSettingsController> _logger;
        private readonly IEmailService _emailService;

        public EmailSettingsController(
            IConfiguration configuration, 
            ILogger<EmailSettingsController> logger,
            IEmailService emailService)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>
        /// Get current email settings
        /// </summary>
        [HttpGet]
        public ActionResult<EmailSettingsResponse> GetEmailSettings()
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                
                var response = new EmailSettingsResponse
                {
                    Provider = emailSettings["Provider"] ?? "SMTP",
                    SmtpHost = emailSettings["SmtpHost"] ?? "",
                    SmtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587"),
                    SmtpUser = emailSettings["SmtpUser"] ?? "",
                    SmtpPass = "***", // Don't expose password
                    FromEmail = emailSettings["FromEmail"] ?? "",
                    FromName = emailSettings["FromName"] ?? "",
                    EnableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "false"),
                    EnableTls = bool.Parse(emailSettings["EnableTls"] ?? "true")
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting email settings");
                return StatusCode(500, new { error = "Failed to get email settings" });
            }
        }

        /// <summary>
        /// Update email settings
        /// </summary>
        [HttpPut]
        public ActionResult UpdateEmailSettings([FromBody] UpdateEmailSettingsRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(request.SmtpHost) || 
                    string.IsNullOrEmpty(request.SmtpUser) || 
                    string.IsNullOrEmpty(request.FromEmail))
                {
                    return BadRequest(new { error = "SMTP Host, User, and From Email are required" });
                }

                // Update Azure App Service settings
                var settings = new Dictionary<string, string>
                {
                    ["EmailSettings__Provider"] = request.Provider ?? "SMTP",
                    ["EmailSettings__SmtpHost"] = request.SmtpHost,
                    ["EmailSettings__SmtpPort"] = request.SmtpPort.ToString(),
                    ["EmailSettings__SmtpUser"] = request.SmtpUser,
                    ["EmailSettings__FromEmail"] = request.FromEmail,
                    ["EmailSettings__FromName"] = request.FromName ?? "Hopewell Clinic",
                    ["EmailSettings__EnableSsl"] = request.EnableSsl.ToString().ToLower(),
                    ["EmailSettings__EnableTls"] = request.EnableTls.ToString().ToLower()
                };

                // Only update password if provided
                if (!string.IsNullOrEmpty(request.SmtpPass) && request.SmtpPass != "***")
                {
                    settings["EmailSettings__SmtpPass"] = request.SmtpPass;
                }

                _logger.LogInformation("Email settings updated by admin: {AdminId}", User.Identity?.Name);

                return Ok(new { message = "Email settings updated successfully. Restart required for changes to take effect." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email settings");
                return StatusCode(500, new { error = "Failed to update email settings" });
            }
        }

        /// <summary>
        /// Test email configuration
        /// </summary>
        [HttpPost("test")]
        public async Task<ActionResult> TestEmailConfiguration([FromBody] TestEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TestEmail))
                {
                    return BadRequest(new { error = "Test email address is required" });
                }

                var result = await _emailService.SendEmailAsync(
                    request.TestEmail,
                    "Hopewell Clinic - Email Configuration Test",
                    "<h2>Email Configuration Test</h2><p>This is a test email to verify the email configuration is working correctly.</p><p>If you receive this email, the configuration is successful!</p>",
                    "Email Configuration Test - This is a test email to verify the email configuration is working correctly. If you receive this email, the configuration is successful!"
                );

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = "Test email sent successfully!",
                        messageId = result.MessageId 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        error = result.Error ?? "Failed to send test email" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing email configuration");
                return StatusCode(500, new { error = "Failed to test email configuration" });
            }
        }

        /// <summary>
        /// Get email configuration status
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetEmailStatus()
        {
            try
            {
                var testResult = await _emailService.TestConfigurationAsync();
                
                return Ok(new
                {
                    IsConfigured = testResult.Success,
                    Status = testResult.Success ? "Ready" : "Not Configured",
                    Message = testResult.Message,
                    LastTested = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting email status");
                return StatusCode(500, new { error = "Failed to get email status" });
            }
        }
    }
}
