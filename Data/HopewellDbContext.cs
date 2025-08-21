using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;

namespace HopewellClinicApi.Data
{
    public class HopewellDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public HopewellDbContext(DbContextOptions<HopewellDbContext> options) : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(u => u.Patient)
                 .WithOne(p => p.User)
                 .HasForeignKey<Patient>(p => p.UserId)
                 .IsRequired(false);

                b.HasOne(u => u.Staff)
                 .WithOne(s => s.User)
                 .HasForeignKey<Staff>(s => s.UserId)
                 .IsRequired(false);
            });

            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique constraints
            modelBuilder.Entity<Patient>().HasIndex(p => p.PatientNumber).IsUnique();
            modelBuilder.Entity<Staff>().HasIndex(s => s.StaffNumber).IsUnique();

            // Property types
            modelBuilder.Entity<TimeSlot>().Property(t => t.StartTime).HasColumnType("time");
            modelBuilder.Entity<TimeSlot>().Property(t => t.EndTime).HasColumnType("time");
            modelBuilder.Entity<Appointment>().Property(a => a.StartTime).HasColumnType("time");
            modelBuilder.Entity<Appointment>().Property(a => a.EndTime).HasColumnType("time");
            modelBuilder.Entity<Appointment>().Property(a => a.AppointmentDate).HasColumnType("date");
            modelBuilder.Entity<Patient>().Property(p => p.DateOfBirth).HasColumnType("date");

            // Seed static data
            DataSeeder.SeedData(modelBuilder);
        }

        // Automatically set CreatedAt and UpdatedAt
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Patient || e.Entity is Staff || e.Entity is Service ||
                            e.Entity is Appointment || e.Entity is TimeSlot || e.Entity is ApplicationUser ||
                            e.Entity is ApplicationRole);

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = now;
                    entry.Property("UpdatedAt").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedAt").CurrentValue = now;
                }
            }
        }
    }
}
