using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        // Optional fields - accept empty strings without validation errors
        public string? Phone { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; } // Flat string format (e.g., "123 Main St, City, State, 12345")
        public string? EmergencyContact { get; set; } // Flat string format
        public string? EmergencyPhone { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateUserResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserApiResponse User { get; set; } = new();
    }
}
