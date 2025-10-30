using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using System.Security.Claims;

namespace HopewellClinicApi.Services
{
    /// <summary>
    /// Service for managing appointment status transitions and business logic
    /// </summary>
    public interface IAppointmentStatusService
    {
        Task<AppointmentStatusResponse> UpdateAppointmentStatusAsync(Guid appointmentId, UpdateAppointmentStatusRequest request, string userId, string userRole);
        Task<AppointmentActionResponse> ApproveAppointmentAsync(Guid appointmentId, ApproveAppointmentRequest request, string userId);
        Task<AppointmentActionResponse> RejectAppointmentAsync(Guid appointmentId, RejectAppointmentRequest request, string userId);
        Task<bool> CanUserModifyAppointmentAsync(Guid appointmentId, string userId, string userRole);
        Task<List<AppointmentAuditLogEntry>> GetAppointmentAuditLogAsync(Guid appointmentId);
        Task LogAppointmentActionAsync(Guid appointmentId, string action, string? oldStatus, string? newStatus, string? reason, string userId, string? details = null);
    }

    public class AppointmentStatusService : IAppointmentStatusService
    {
        private readonly HopewellDbContext _context;

        public AppointmentStatusService(HopewellDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates appointment status with validation and audit logging
        /// </summary>
        public async Task<AppointmentStatusResponse> UpdateAppointmentStatusAsync(Guid appointmentId, UpdateAppointmentStatusRequest request, string userId, string userRole)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                throw new ArgumentException("Appointment not found");
            }

            // Check if user can modify this appointment
            if (!await CanUserModifyAppointmentAsync(appointmentId, userId, userRole))
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this appointment");
            }

            // Validate status transition
            if (!IsValidStatusTransition(appointment.Status, request.Status, userRole))
            {
                throw new InvalidOperationException($"Invalid status transition from '{appointment.Status}' to '{request.Status}'");
            }

            var oldStatus = appointment.Status;
            appointment.Status = request.Status;
            appointment.UpdatedAt = DateTime.UtcNow;

            // Add rejection reason to notes if provided
            if (!string.IsNullOrEmpty(request.Reason) && request.Status == "cancelled")
            {
                var rejectionNote = $"Rejected by {userRole}: {request.Reason}";
                appointment.Notes = string.IsNullOrEmpty(appointment.Notes) 
                    ? rejectionNote 
                    : $"{appointment.Notes}\n{rejectionNote}";
            }

            await _context.SaveChangesAsync();

            // Log the action
            await LogAppointmentActionAsync(appointmentId, "status_updated", oldStatus, request.Status, request.Reason, userId);

            return new AppointmentStatusResponse
            {
                Id = appointment.Id,
                Status = appointment.Status,
                UpdatedAt = appointment.UpdatedAt,
                Message = "Appointment status updated successfully",
                Reason = request.Reason,
                UpdatedBy = userId
            };
        }

        /// <summary>
        /// Approves an appointment (sets status to confirmed)
        /// </summary>
        public async Task<AppointmentActionResponse> ApproveAppointmentAsync(Guid appointmentId, ApproveAppointmentRequest request, string userId)
        {
            var statusRequest = new UpdateAppointmentStatusRequest
            {
                Status = "confirmed",
                Reason = request.Notes
            };

            var result = await UpdateAppointmentStatusAsync(appointmentId, statusRequest, userId, "admin");

            return new AppointmentActionResponse
            {
                Id = result.Id,
                Status = result.Status,
                Action = "approved",
                Message = "Appointment approved successfully",
                UpdatedAt = result.UpdatedAt,
                Reason = result.Reason,
                UpdatedBy = result.UpdatedBy
            };
        }

        /// <summary>
        /// Rejects an appointment (sets status to cancelled with reason)
        /// </summary>
        public async Task<AppointmentActionResponse> RejectAppointmentAsync(Guid appointmentId, RejectAppointmentRequest request, string userId)
        {
            var statusRequest = new UpdateAppointmentStatusRequest
            {
                Status = "cancelled",
                Reason = request.Reason
            };

            var result = await UpdateAppointmentStatusAsync(appointmentId, statusRequest, userId, "admin");

            // Add additional notes if provided
            if (!string.IsNullOrEmpty(request.AdditionalNotes))
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment != null)
                {
                    appointment.Notes = string.IsNullOrEmpty(appointment.Notes) 
                        ? request.AdditionalNotes 
                        : $"{appointment.Notes}\n{request.AdditionalNotes}";
                    await _context.SaveChangesAsync();
                }
            }

            return new AppointmentActionResponse
            {
                Id = result.Id,
                Status = result.Status,
                Action = "rejected",
                Message = "Appointment rejected successfully",
                UpdatedAt = result.UpdatedAt,
                Reason = result.Reason,
                UpdatedBy = result.UpdatedBy
            };
        }

        /// <summary>
        /// Checks if a user can modify a specific appointment
        /// </summary>
        public async Task<bool> CanUserModifyAppointmentAsync(Guid appointmentId, string userId, string userRole)
        {
            // Admins can modify any appointment
            if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Doctors can only modify their own appointments
            if (userRole.Equals("doctor", StringComparison.OrdinalIgnoreCase))
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == appointmentId && a.StaffId.ToString() == userId);
                return appointment != null;
            }

            // Other roles cannot modify appointments
            return false;
        }

        /// <summary>
        /// Gets audit log for an appointment
        /// </summary>
        public async Task<List<AppointmentAuditLogEntry>> GetAppointmentAuditLogAsync(Guid appointmentId)
        {
            var auditLogs = await _context.AppointmentAuditLogs
                .Where(log => log.AppointmentId == appointmentId)
                .OrderByDescending(log => log.PerformedAt)
                .Select(log => new AppointmentAuditLogEntry
                {
                    Id = log.Id,
                    AppointmentId = log.AppointmentId,
                    Action = log.Action,
                    OldStatus = log.OldStatus,
                    NewStatus = log.NewStatus,
                    Reason = log.Reason,
                    PerformedBy = log.PerformedBy.ToString(),
                    PerformedAt = log.PerformedAt,
                    Details = log.Details
                })
                .ToListAsync();

            return auditLogs;
        }

        /// <summary>
        /// Logs an appointment action for audit purposes
        /// </summary>
        public async Task LogAppointmentActionAsync(Guid appointmentId, string action, string? oldStatus, string? newStatus, string? reason, string userId, string? details = null)
        {
            var auditLog = new AppointmentAuditLog
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointmentId,
                Action = action,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Reason = reason,
                PerformedBy = Guid.Parse(userId),
                PerformedAt = DateTime.UtcNow,
                Details = details
            };

            _context.AppointmentAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Validates if a status transition is allowed
        /// </summary>
        private bool IsValidStatusTransition(string currentStatus, string newStatus, string userRole)
        {
            var validTransitions = new Dictionary<string, string[]>
            {
                ["pending"] = new[] { "confirmed", "cancelled" },
                ["confirmed"] = new[] { "cancelled", "completed" },
                ["cancelled"] = new[] { "pending" }, // Allow rescheduling
                ["completed"] = new string[] { }, // No transitions from completed
                ["walkin"] = new[] { "completed", "cancelled" }
            };

            // Admins can perform any transition
            if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Other roles follow standard transition rules
            return validTransitions.ContainsKey(currentStatus) && 
                   validTransitions[currentStatus].Contains(newStatus);
        }
    }
}
