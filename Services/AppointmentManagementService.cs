using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using System.Text.Json;

namespace HopewellClinicApi.Services
{
    public interface IAppointmentManagementService
    {
        Task<ValidationResult> ValidateAppointmentUpdateAsync(Guid appointmentId, AdminUpdateAppointmentRequest request);
        Task<AppointmentOperationResponse> UpdateAppointmentAsync(Guid appointmentId, AdminUpdateAppointmentRequest request, Guid adminId);
        Task<AppointmentOperationResponse> DeleteAppointmentAsync(Guid appointmentId, Guid adminId);
        Task<PagedAppointmentsResponse> GetAppointmentsWithPaginationAsync(AppointmentSearchRequest request);
        Task<AppointmentDetailDto?> GetAppointmentByIdAsync(Guid appointmentId);
        Task LogAppointmentChangeAsync(Guid appointmentId, string action, Guid changedBy, Appointment? oldAppointment, Appointment? newAppointment);
    }

    public class AppointmentManagementService : IAppointmentManagementService
    {
        private readonly HopewellDbContext _context;
        private readonly ILogger<AppointmentManagementService> _logger;

        public AppointmentManagementService(HopewellDbContext context, ILogger<AppointmentManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateAppointmentUpdateAsync(Guid appointmentId, AdminUpdateAppointmentRequest request)
        {
            var errors = new List<string>();

            // Date validation
            if (!ValidateAppointmentDate(request.AppointmentDate))
            {
                errors.Add("Cannot book appointments in the past or more than 30 days in advance");
            }

            // Time validation
            if (!ValidateAppointmentTime(request.StartTime, request.EndTime))
            {
                errors.Add("End time must be after start time");
            }

            // Doctor availability check
            if (request.StaffId.HasValue)
            {
                if (!await IsDoctorAvailableAsync(request.StaffId.Value, request.AppointmentDate, request.StartTime, request.EndTime))
                {
                    errors.Add("Doctor is not available at the selected time");
                }

                // Conflict detection
                if (await HasAppointmentConflictAsync(request.StaffId.Value, request.AppointmentDate, request.StartTime, request.EndTime, appointmentId))
                {
                    errors.Add("Doctor has a conflicting appointment at this time");
                }
            }

            // Service validation
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.IsActive);
            if (service == null)
            {
                errors.Add("Selected service is not available");
            }

            // Status validation
            var validStatuses = new[] { "pending", "confirmed", "cancelled", "completed" };
            if (!validStatuses.Contains(request.Status.ToLower()))
            {
                errors.Add("Invalid appointment status");
            }

            return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
        }

        public async Task<AppointmentOperationResponse> UpdateAppointmentAsync(Guid appointmentId, AdminUpdateAppointmentRequest request, Guid adminId)
        {
            try
            {
                var existingAppointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Staff)
                    .Include(a => a.Service)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (existingAppointment == null)
                {
                    return new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = "Appointment not found"
                    };
                }

                // Store old values for audit
                var oldAppointment = CloneAppointment(existingAppointment);

                // Update appointment
                existingAppointment.AppointmentDate = request.AppointmentDate.Date;
                existingAppointment.StartTime = TimeOnly.Parse(request.StartTime);
                existingAppointment.EndTime = TimeOnly.Parse(request.EndTime);
                existingAppointment.Status = request.Status.ToLower();
                existingAppointment.StaffId = request.StaffId;
                existingAppointment.ServiceId = request.ServiceId;
                existingAppointment.Notes = request.Notes;
                existingAppointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Log audit trail
                // await LogAppointmentChangeAsync(appointmentId, "updated", adminId, oldAppointment, existingAppointment);

                return new AppointmentOperationResponse
                {
                    Success = true,
                    Message = "Appointment updated successfully",
                    Id = appointmentId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", appointmentId);
                return new AppointmentOperationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                };
            }
        }

        public async Task<AppointmentOperationResponse> DeleteAppointmentAsync(Guid appointmentId, Guid adminId)
        {
            try
            {
                var existingAppointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Staff)
                    .Include(a => a.Service)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (existingAppointment == null)
                {
                    return new AppointmentOperationResponse
                    {
                        Success = false,
                        Error = "Appointment not found"
                    };
                }

                // Log audit trail before deletion
                // await LogAppointmentChangeAsync(appointmentId, "deleted", adminId, existingAppointment, null);

                _context.Appointments.Remove(existingAppointment);
                await _context.SaveChangesAsync();

                return new AppointmentOperationResponse
                {
                    Success = true,
                    Message = "Appointment deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", appointmentId);
                return new AppointmentOperationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                };
            }
        }

        public async Task<PagedAppointmentsResponse> GetAppointmentsWithPaginationAsync(AppointmentSearchRequest request)
        {
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                    .ThenInclude(s => s.User)
                    .Include(a => a.Service)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(request.Search))
                {
                    var searchTerm = request.Search.ToLower();
                    query = query.Where(a =>
                        a.Patient.User.FirstName.ToLower().Contains(searchTerm) ||
                        a.Patient.User.LastName.ToLower().Contains(searchTerm) ||
                        a.Patient.User.Email.ToLower().Contains(searchTerm) ||
                        a.Service.Name.ToLower().Contains(searchTerm) ||
                        (a.Staff != null && (
                            a.Staff.User.FirstName.ToLower().Contains(searchTerm) ||
                            a.Staff.User.LastName.ToLower().Contains(searchTerm)
                        ))
                    );
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(request.Status) && request.Status.ToLower() != "all")
                {
                    query = query.Where(a => a.Status.ToLower() == request.Status.ToLower());
                }

                // Apply date range filter
                if (request.StartDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate <= request.EndDate.Value);
                }

                // Apply doctor filter
                if (request.DoctorId.HasValue)
                {
                    query = query.Where(a => a.StaffId == request.DoctorId.Value);
                }

                // Apply service filter
                if (request.ServiceId.HasValue)
                {
                    query = query.Where(a => a.ServiceId == request.ServiceId.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination
                var appointments = await query
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .Select(a => new AppointmentDetailDto
                    {
                        Id = a.Id,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime.ToString("HH:mm"),
                        EndTime = a.EndTime.ToString("HH:mm"),
                        Status = a.Status,
                        Notes = a.Notes,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        PatientId = a.PatientId,
                        PatientFirstName = a.Patient.User.FirstName,
                        PatientLastName = a.Patient.User.LastName,
                        PatientEmail = a.Patient.User.Email ?? string.Empty,
                        PatientPhone = a.Patient.User.PhoneNumber ?? string.Empty,
                        StaffId = a.StaffId,
                        DoctorFirstName = a.Staff != null ? a.Staff.User.FirstName : null,
                        DoctorLastName = a.Staff != null ? a.Staff.User.LastName : null,
                        DoctorEmail = a.Staff != null ? a.Staff.User.Email : null,
                        ServiceId = a.ServiceId,
                        ServiceName = a.Service.Name,
                        ServiceDescription = a.Service.Description ?? string.Empty,
                        ServicePrice = a.ServicePrice
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.Limit);

                return new PagedAppointmentsResponse
                {
                    Success = true,
                    Appointments = appointments,
                    TotalCount = totalCount,
                    CurrentPage = request.Page,
                    TotalPages = totalPages,
                    HasNextPage = request.Page < totalPages,
                    HasPreviousPage = request.Page > 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments with pagination");
                return new PagedAppointmentsResponse
                {
                    Success = false,
                    Error = "Internal server error"
                };
            }
        }

        public async Task<AppointmentDetailDto?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Include(a => a.Staff)
                    .ThenInclude(s => s.User)
                    .Include(a => a.Service)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                    return null;

                return new AppointmentDetailDto
                {
                    Id = appointment.Id,
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime.ToString("HH:mm"),
                    EndTime = appointment.EndTime.ToString("HH:mm"),
                    Status = appointment.Status,
                    Notes = appointment.Notes,
                    CreatedAt = appointment.CreatedAt,
                    UpdatedAt = appointment.UpdatedAt,
                    PatientId = appointment.PatientId,
                    PatientFirstName = appointment.Patient.User.FirstName,
                    PatientLastName = appointment.Patient.User.LastName,
                    PatientEmail = appointment.Patient.User.Email ?? string.Empty,
                    PatientPhone = appointment.Patient.User.PhoneNumber ?? string.Empty,
                    StaffId = appointment.StaffId,
                    DoctorFirstName = appointment.Staff?.User.FirstName,
                    DoctorLastName = appointment.Staff?.User.LastName,
                    DoctorEmail = appointment.Staff?.User.Email,
                    ServiceId = appointment.ServiceId,
                    ServiceName = appointment.Service.Name,
                    ServiceDescription = appointment.Service.Description ?? string.Empty,
                    ServicePrice = appointment.ServicePrice
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment {AppointmentId}", appointmentId);
                return null;
            }
        }

        public async Task LogAppointmentChangeAsync(Guid appointmentId, string action, Guid changedBy, Appointment? oldAppointment, Appointment? newAppointment)
        {
            try
            {
                var auditLog = new AppointmentAuditLog
                {
                    AppointmentId = appointmentId,
                    Action = action,
                    PerformedBy = changedBy,
                    PerformedAt = DateTime.UtcNow,
                    Details = oldAppointment != null ? JsonSerializer.Serialize(oldAppointment) : null
                };

                _context.AppointmentAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging appointment change for {AppointmentId}", appointmentId);
            }
        }

        private bool ValidateAppointmentDate(DateTime appointmentDate)
        {
            var today = DateTime.Today;
            var maxDate = today.AddDays(30);
            
            return appointmentDate >= today && appointmentDate <= maxDate;
        }

        private bool ValidateAppointmentTime(string startTime, string endTime)
        {
            if (!TimeOnly.TryParse(startTime, out var start) || !TimeOnly.TryParse(endTime, out var end))
                return false;
            
            return end > start;
        }

        private async Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime appointmentDate, string startTime, string endTime)
        {
            try
            {
                var dayOfWeek = appointmentDate.DayOfWeek.ToString();
                
                var shiftSchedule = await _context.ShiftSchedules
                    .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.IsActive);

                if (shiftSchedule == null)
                    return false;

                var appointmentStart = TimeOnly.Parse(startTime);
                var appointmentEnd = TimeOnly.Parse(endTime);

                return appointmentStart >= TimeOnly.FromTimeSpan(shiftSchedule.StartTime) && 
                       appointmentEnd <= TimeOnly.FromTimeSpan(shiftSchedule.EndTime);
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> HasAppointmentConflictAsync(Guid doctorId, DateTime appointmentDate, string startTime, string endTime, Guid? excludeAppointmentId = null)
        {
            try
            {
                var appointmentStart = TimeOnly.Parse(startTime);
                var appointmentEnd = TimeOnly.Parse(endTime);

                var existingAppointments = await _context.Appointments
                    .Where(a => a.StaffId == doctorId && 
                               a.AppointmentDate == appointmentDate.Date &&
                               a.Id != excludeAppointmentId)
                    .ToListAsync();

                return existingAppointments.Any(apt => 
                    !(appointmentEnd <= apt.StartTime || appointmentStart >= apt.EndTime)
                );
            }
            catch
            {
                return true; // Assume conflict if we can't check
            }
        }

        private Appointment CloneAppointment(Appointment appointment)
        {
            return new Appointment
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                ServiceId = appointment.ServiceId,
                StaffId = appointment.StaffId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                BookingType = appointment.BookingType,
                Notes = appointment.Notes,
                ApprovalStatus = appointment.ApprovalStatus,
                RejectionReason = appointment.RejectionReason,
                ApprovedAt = appointment.ApprovedAt,
                ApprovedBy = appointment.ApprovedBy,
                ApprovedByNurseId = appointment.ApprovedByNurseId,
                NurseApprovalDate = appointment.NurseApprovalDate,
                ApprovalNotes = appointment.ApprovalNotes,
                IsWalkIn = appointment.IsWalkIn,
                ServicePrice = appointment.ServicePrice,
                PaymentStatus = appointment.PaymentStatus,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
        }
    }
}
