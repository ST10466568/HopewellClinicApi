using System;
using System.Linq;
using HopewellClinicApi.Models;
using HopewellClinicApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "admin", NormalizedName = "ADMIN" });
        }
        if (!await roleManager.RoleExistsAsync("doctor"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "doctor", NormalizedName = "DOCTOR" });
        }
        if (!await roleManager.RoleExistsAsync("patient"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "patient", NormalizedName = "PATIENT" });
        }

        // Seed Admin User
        if (await userManager.FindByEmailAsync("admin@hopewell.com") == null)
        {
            var adminUser = new ApplicationUser 
            { 
                UserName = "admin", 
                Email = "admin@hopewell.com", 
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }

        // Seed Doctor User
        if (await userManager.FindByEmailAsync("doctor@hopewell.com") == null)
        {
            var doctorUser = new ApplicationUser 
            { 
                UserName = "doctor", 
                Email = "doctor@hopewell.com", 
                EmailConfirmed = true,
                FirstName = "Dr. John",
                LastName = "Smith",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(doctorUser, "Doctor@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(doctorUser, "doctor");
            }
        }

        // Seed Patient User
        if (await userManager.FindByEmailAsync("patient@hopewell.com") == null)
        {
            var patientUser = new ApplicationUser 
            { 
                UserName = "patient", 
                Email = "patient@hopewell.com", 
                EmailConfirmed = true,
                FirstName = "Jane",
                LastName = "Doe",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(patientUser, "Patient@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(patientUser, "patient");
            }
        }
    }

    public static async Task SeedPatients(HopewellDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Get the patient user
        var patientUser = await userManager.FindByEmailAsync("patient@hopewell.com");
        if (patientUser != null)
        {
            // Update patient user with proper names if they're empty
            if (string.IsNullOrEmpty(patientUser.FirstName) || string.IsNullOrEmpty(patientUser.LastName))
            {
                patientUser.FirstName = "Jane";
                patientUser.LastName = "Doe";
                patientUser.UpdatedAt = DateTime.UtcNow;
                await userManager.UpdateAsync(patientUser);
            }

            // Check if patient record already exists
            var existingPatient = await context.Patients
                .FirstOrDefaultAsync(p => p.UserId == patientUser.Id);
            
            if (existingPatient == null)
            {
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    UserId = patientUser.Id,
                    PatientNumber = "P001",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Address = "123 Main Street, City, State",
                    EmergencyContactName = "Emergency Contact",
                    EmergencyContactPhone = "555-0123",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Patients.Add(patient);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedStaff(HopewellDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Get the admin user
        var adminUser = await userManager.FindByEmailAsync("admin@hopewell.com");
        if (adminUser != null)
        {
            // Update admin user with proper names if they're empty
            if (string.IsNullOrEmpty(adminUser.FirstName) || string.IsNullOrEmpty(adminUser.LastName))
            {
                adminUser.FirstName = "Admin";
                adminUser.LastName = "User";
                adminUser.UpdatedAt = DateTime.UtcNow;
                await userManager.UpdateAsync(adminUser);
            }

            // Check if staff record already exists
            var existingStaff = await context.Staff
                .FirstOrDefaultAsync(s => s.UserId == adminUser.Id);
            
            if (existingStaff == null)
            {
                var staff = new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    StaffNumber = "STF001",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Staff.Add(staff);
                await context.SaveChangesAsync();
            }
        }

        // Get the doctor user
        var doctorUser = await userManager.FindByEmailAsync("doctor@hopewell.com");
        if (doctorUser != null)
        {
            // Update doctor user with proper names if they're empty
            if (string.IsNullOrEmpty(doctorUser.FirstName) || string.IsNullOrEmpty(doctorUser.LastName))
            {
                doctorUser.FirstName = "Dr. John";
                doctorUser.LastName = "Smith";
                doctorUser.UpdatedAt = DateTime.UtcNow;
                await userManager.UpdateAsync(doctorUser);
            }

            // Check if staff record already exists
            var existingStaff = await context.Staff
                .FirstOrDefaultAsync(s => s.UserId == doctorUser.Id);
            
            if (existingStaff == null)
            {
                var staff = new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = doctorUser.Id,
                    StaffNumber = "STF002",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Staff.Add(staff);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedServices(HopewellDbContext context)
    {
        // Check if services already exist
        if (!await context.Services.AnyAsync())
        {
            var services = new List<Service>
            {
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "General Consultation",
                    Description = "General medical consultation with a doctor",
                    DurationMinutes = 30,
                    Price = 150.00m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Follow-up Consultation",
                    Description = "Follow-up medical consultation",
                    DurationMinutes = 20,
                    Price = 100.00m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Emergency Consultation",
                    Description = "Emergency medical consultation",
                    DurationMinutes = 45,
                    Price = 200.00m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }
    }
}