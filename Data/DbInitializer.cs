using System;
using System.Linq;
using HopewellClinicApi.Models;           
using HopewellClinicApi.Data;      

public static class DbInitializer
{
    public static void SeedRuntimeData(HopewellDbContext context)
    {
        var now = DateTime.UtcNow;

        foreach (var user in context.Users.Where(u => u.CreatedAt == default))
        {
            user.CreatedAt = now;
            user.UpdatedAt = now;
        }

        foreach (var service in context.Services.Where(s => s.CreatedAt == default))
        {
            service.CreatedAt = now;
            service.UpdatedAt = now;
        }

        // Seed Patients with dynamic DateOfBirth
        if (!context.Patients.Any())
        {
            context.Patients.Add(new Patient
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655442000"),
                UserId = Guid.Parse("550e8400-e29b-41d4-a716-446655442010"),
                PatientNumber = "PAT001",
                DateOfBirth = new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Address = "123 Main Street, Qunu Village",
                EmergencyContactName = "Sizani Gcaba",
                EmergencyContactPhone = "+27821234568"
            });
        }

        context.SaveChanges();
    }
}
