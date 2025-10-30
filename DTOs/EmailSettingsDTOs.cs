namespace HopewellClinicApi.DTOs
{
    public class EmailSettingsResponse
    {
        public string Provider { get; set; } = "";
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = "";
        public string SmtpPass { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
        public bool EnableSsl { get; set; } = false;
        public bool EnableTls { get; set; } = true;
    }

    public class UpdateEmailSettingsRequest
    {
        public string Provider { get; set; } = "SMTP";
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = "";
        public string SmtpPass { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
        public bool EnableSsl { get; set; } = false;
        public bool EnableTls { get; set; } = true;
    }

    public class TestEmailRequest
    {
        public string TestEmail { get; set; } = "";
    }
}




