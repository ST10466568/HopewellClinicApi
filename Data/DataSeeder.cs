using Microsoft.EntityFrameworkCore;      // <-- Needed for ModelBuilder
using HopewellClinicApi.Models;           // <-- Your entity models
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Static roles
        var roles = new[]
        {
            new ApplicationRole { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655449001"), Name = "admin", NormalizedName = "ADMIN" },
            new ApplicationRole { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655449002"), Name = "doctor", NormalizedName = "DOCTOR" },
            new ApplicationRole { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655449003"), Name = "nurse", NormalizedName = "NURSE" },
            new ApplicationRole { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655449004"), Name = "patient", NormalizedName = "PATIENT" }
        };
        modelBuilder.Entity<ApplicationRole>().HasData(roles);

        // Static users (pre-hashed password)
        const string passwordHash = "AQAAAAIAAYagAAAAEBLC+82KkL8Zl2E1f4aXkXwzWf6a/b8b/eXwzWf6a/b8b/eXwzWf6a/b8b/eQ==";

        var drNomsaUser = new ApplicationUser
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655441001"),
            FirstName = "Dr. Nomsa",
            LastName = "Mandela",
            Email = "nomsa.mandela@hopewell.com",
            NormalizedEmail = "NOMSA.MANDELA@HOPEWELL.COM",
            UserName = "nomsa.mandela@hopewell.com",
            NormalizedUserName = "NOMSA.MANDELA@HOPEWELL.COM",
            PhoneNumber = "+27123456789",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            PasswordHash = passwordHash
        };

        var drThaboUser = new ApplicationUser
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655441003"),
            FirstName = "Dr. Thabo",
            LastName = "Sithole",
            Email = "thabo.sithole@hopewell.com",
            NormalizedEmail = "THABO.SITHOLE@HOPEWELL.COM",
            UserName = "thabo.sithole@hopewell.com",
            NormalizedUserName = "THABO.SITHOLE@HOPEWELL.COM",
            PhoneNumber = "+27123456790",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            PasswordHash = passwordHash
        };

        modelBuilder.Entity<ApplicationUser>().HasData(drNomsaUser, drThaboUser);

        // User roles
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid> { UserId = drNomsaUser.Id, RoleId = roles.First(r => r.Name == "doctor").Id },
            new IdentityUserRole<Guid> { UserId = drThaboUser.Id, RoleId = roles.First(r => r.Name == "doctor").Id }
        );

        // Services
        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                Name = "General Consultation",
                Description = "General medical consultation and examination",
                DurationMinutes = 30,
                IsActive = true
            },
            new Service
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                Name = "Pediatrics Consultation",
                Description = "Health services for infants and children",
                DurationMinutes = 30,
                IsActive = true
            },
            new Service
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
                Name = "Dental Checkup",
                Description = "Routine dental examination and cleaning",
                DurationMinutes = 45,
                IsActive = true
            },
            new Service
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440003"),
                Name = "Physiotherapy Session",
                Description = "Physical therapy for rehabilitation",
                DurationMinutes = 60,
                IsActive = true
            },
            new Service
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440004"),
                Name = "Vaccination",
                Description = "Routine immunizations and boosters",
                DurationMinutes = 20,
                IsActive = true
            }
        );

        // Staff
        modelBuilder.Entity<Staff>().HasData(
            new Staff
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655441000"),
                UserId = drNomsaUser.Id,
                StaffNumber = "DOC001"
            },
            new Staff
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655441002"),
                UserId = drThaboUser.Id,
                StaffNumber = "DOC002"
            }
        );

        // TimeSlots (Mon-Fri: 09:00-12:00, hourly)
        modelBuilder.Entity<TimeSlot>().HasData(
            // Monday
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443101"), DayOfWeek = 1, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443102"), DayOfWeek = 1, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443103"), DayOfWeek = 1, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsActive = true, IsBooked = false },
            // Tuesday
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443201"), DayOfWeek = 2, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443202"), DayOfWeek = 2, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443203"), DayOfWeek = 2, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsActive = true, IsBooked = false },
            // Wednesday
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443301"), DayOfWeek = 3, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443302"), DayOfWeek = 3, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443303"), DayOfWeek = 3, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsActive = true, IsBooked = false },
            // Thursday
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443401"), DayOfWeek = 4, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443402"), DayOfWeek = 4, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443403"), DayOfWeek = 4, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsActive = true, IsBooked = false },
            // Friday
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443501"), DayOfWeek = 5, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443502"), DayOfWeek = 5, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsActive = true, IsBooked = false },
            new TimeSlot { Id = Guid.Parse("550e8400-e29b-41d4-a716-446655443503"), DayOfWeek = 5, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsActive = true, IsBooked = false }
        );

        // ❌ Do NOT seed Patients here (dynamic DateOfBirth)
    }
}
