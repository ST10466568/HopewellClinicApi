using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Services
{
    public class UserService
    {
        private readonly HopewellDbContext _context;

        public UserService(HopewellDbContext context)
        {
            _context = context;
        }

        public async Task<(UserResponse user, PatientResponse? patient, StaffResponse? staff)> GetUserSessionAsync(Guid userId)
        {
            // Try to find as patient first
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                var userResponse = new UserResponse
                {
                    Id = userId,
                    Email = patient.User.Email ?? "",
                    UserType = "patient"
                };

                var patientResponse = new PatientResponse
                {
                    Id = patient.Id,
                    UserId = patient.UserId,
                    PatientNumber = patient.PatientNumber,
                    FirstName = patient.User.FirstName,
                    LastName = patient.User.LastName,
                    Phone = patient.User.PhoneNumber,
                    Email = patient.User.Email,
                    DateOfBirth = patient.DateOfBirth,
                    Address = patient.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    // CreatedAt = patient.CreatedAt, // Property does not exist on Patient model
                    // UpdatedAt = patient.UpdatedAt  // Property does not exist on Patient model
                };

                return (userResponse, patientResponse, null);
            }

            // Try to find as staff
            var staff = await _context.Staff
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (staff != null)
            {
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == staff.UserId);
                var roleName = "staff"; // default
                if (userRole != null)
                {
                    var role = await _context.Roles.FindAsync(userRole.RoleId);
                    if (role != null)
                    {
                        roleName = role.Name ?? roleName;
                    }
                }

                var userResponse = new UserResponse
                {
                    Id = userId,
                    Email = staff.User.Email ?? "",
                    UserType = "staff"
                };

                var staffResponse = new StaffResponse
                {
                    Id = staff.Id,
                    UserId = staff.UserId,
                    StaffNumber = staff.StaffNumber,
                    FirstName = staff.User.FirstName,
                    LastName = staff.User.LastName,
                    Role = roleName,
                    Phone = staff.User.PhoneNumber,
                    IsActive = staff.User.IsActive,
                    // CreatedAt = staff.CreatedAt, // Property does not exist on Staff model
                    // UpdatedAt = staff.UpdatedAt  // Property does not exist on Staff model
                };

                return (userResponse, null, staffResponse);
            }

            throw new InvalidOperationException("User not found");
        }

        public async Task<Patient?> FindPatientByEmailAsync(string email)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.User.Email == email);
        }

        public async Task<bool> PatientExistsAsync(string email)
        {
            return await _context.Patients
                .AnyAsync(p => p.User.Email == email);
        }
    }
}