using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    // Enhanced Appointment DTOs
    public class CreateAppointmentRequestEnhanced
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;

        public Guid? StaffId { get; set; }

        public string? Notes { get; set; }
    }

    // Patient Management DTOs
    public class UpdatePatientRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PhoneNumber { get; set; } // Backward compatibility
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Address { get; set; } // Backward compatibility - will be populated from AddressObject if needed
        [System.Text.Json.Serialization.JsonPropertyName("address")]
        public AddressDto? AddressObject { get; set; } // Accepts nested object (frontend sends "address")
        [System.Text.Json.Serialization.JsonPropertyName("emergencyContact")]
        public EmergencyContactDto? EmergencyContact { get; set; }
    }



    public class PatientSummaryDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PatientNumber { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    // Service Management DTOs
    public class CreateServiceRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int DurationMinutes { get; set; }
    }

    public class UpdateServiceRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DurationMinutes { get; set; }
        public bool? IsActive { get; set; }
    }

    // Staff Management DTOs
    public class CreateStaffRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        // Optional fields
        public string? Phone { get; set; }
        
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in YYYY-MM-DD format")]
        public string? DateOfBirth { get; set; }
        
        public string? Address { get; set; }
        
        public string? EmergencyContact { get; set; }
        
        public string? EmergencyPhone { get; set; }
    }

    // Enhanced Patient Creation DTOs
    public class CreatePatientRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "patient";

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date of birth must be in YYYY-MM-DD format")]
        public string DateOfBirth { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string EmergencyContact { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string EmergencyPhone { get; set; } = string.Empty;
    }

    // Unified User Creation DTO
    public class CreateUserRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        // Optional patient-specific fields
        public string? Phone { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
    }

    public class UpdateStaffRequest
    {
        public string? PhoneNumber { get; set; }
    }

    // Response DTOs
    public class UserCreationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string? Error { get; set; }
    }


    // Admin Walk-in Appointment DTO
    public class AdminWalkInAppointmentRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Appointment date must be in YYYY-MM-DD format")]
        public string AppointmentDate { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{2}:\d{2}(:\d{2})?$", ErrorMessage = "Start time must be in HH:mm or HH:mm:ss format")]
        public string StartTime { get; set; } = string.Empty;

        [RegularExpression(@"^\d{2}:\d{2}(:\d{2})?$", ErrorMessage = "End time must be in HH:mm or HH:mm:ss format")]
        public string? EndTime { get; set; }

        public string? Notes { get; set; }
    }

    public class UpdateAvailabilityRequest
    {
        [Required]
        public int DayOfWeek { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        public string EndTime { get; set; } = string.Empty;
    }

    // Doctor Dashboard DTOs
    public class CreateWalkinAppointmentDto
    {
        [Required]
        public string PatientFirstName { get; set; } = string.Empty;

        [Required]
        public string PatientLastName { get; set; } = string.Empty;

        [Required]
        public string PatientPhone { get; set; } = string.Empty;

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;
    }

    public class UpdateStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    // Nurse Dashboard DTOs
    public class BookAppointmentForPatientDto
    {
        [Required]
        public Guid PatientId { get; set; }

        public Guid? StaffId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;
    }

    // Admin Dashboard DTOs
    public class UpdateUserRoleDto
    {
        [Required]
        public string NewRole { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentStatsDto
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalAppointments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ServiceRevenueDto> ServiceBreakdown { get; set; } = new List<ServiceRevenueDto>();
        public List<RevenueByMonthDto> RevenueByMonth { get; set; } = new List<RevenueByMonthDto>();
        public List<RevenueByWeekDto> RevenueByWeek { get; set; } = new List<RevenueByWeekDto>();
    }

    public class ServiceRevenueDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public decimal Revenue { get; set; }
    }

    // Revenue breakdown DTOs
    public class RevenueByMonthDto
    {
        public string Month { get; set; } = string.Empty; // yyyy-MM
        public decimal Revenue { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class RevenueByWeekDto
    {
        public string WeekStart { get; set; } = string.Empty; // yyyy-MM-dd (Monday)
        public decimal Revenue { get; set; }
        public int AppointmentCount { get; set; }
    }

    // Service usage analytics
    public class ServiceUsageItemDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public double PercentageOfTotal { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class DateRangeDto
    {
        public string StartDate { get; set; } = string.Empty; // ISO yyyy-MM-dd
        public string EndDate { get; set; } = string.Empty;   // ISO yyyy-MM-dd
    }

    public class ServiceUsageReportDto
    {
        public List<ServiceUsageItemDto> Services { get; set; } = new List<ServiceUsageItemDto>();
        public int TotalAppointments { get; set; }
        public DateRangeDto DateRange { get; set; } = new DateRangeDto();
    }

    // Comprehensive analytics response
    public class ComprehensiveAnalyticsDto
    {
        public AppointmentStatsDto AppointmentStats { get; set; } = new AppointmentStatsDto();
        public List<ServiceUsageItemDto> ServiceUsage { get; set; } = new List<ServiceUsageItemDto>();
        public RevenueReportDto RevenueData { get; set; } = new RevenueReportDto();
        public DateRangeDto DateRange { get; set; } = new DateRangeDto();
        public DateTime GeneratedAt { get; set; }
    }

    // Standard API error
    public class ApiErrorDto
    {
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    // User Profile Update DTOs
    public class AddressDto
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }

    public class EmergencyContactDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Relationship { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public AddressDto? Address { get; set; }

        public EmergencyContactDto? EmergencyContact { get; set; }
    }

    public class ProfileUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProfileDataDto? Data { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
    }

    public class ProfileDataDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public AddressDto? Address { get; set; }
        public EmergencyContactDto? EmergencyContact { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
