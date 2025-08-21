using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    public class RegisterPatientDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
    }
}

