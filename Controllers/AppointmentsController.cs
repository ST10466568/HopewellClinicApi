using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using HopewellClinicApi.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(HopewellDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                var user = await _userManager.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                if (user.Patient != null)
                {
                    var appointments = await _context.Appointments
                        .Include(a => a.Service)
                        .Where(a => a.PatientId == user.Patient.Id)
                        .Select(a => new AppointmentResponse
                        {
                            Id = a.Id,
                            AppointmentDate = a.AppointmentDate,
                            StartTime = a.StartTime,
                            EndTime = a.EndTime,
                            Status = a.Status,
                            Notes = a.Notes,
                            Service = new ServiceResponse
                            {
                                Id = a.Service.Id,
                                Name = a.Service.Name,
                                Description = a.Service.Description,
                                DurationMinutes = a.Service.DurationMinutes
                            }
                        })
                        .ToListAsync();

                    return Ok(appointments);
                }
                else if (user.Staff != null)
                {
                    var appointments = await _context.Appointments
                        .Include(a => a.Service)
                        .Include(a => a.Patient)
                        .Where(a => a.StaffId == user.Staff.Id)
                        .Select(a => new AppointmentResponse
                        {
                            Id = a.Id,
                            AppointmentDate = a.AppointmentDate,
                            StartTime = a.StartTime,
                            EndTime = a.EndTime,
                            Status = a.Status,
                            Notes = a.Notes,
                            Patient = new PatientResponse
                            {
                                Id = a.Patient.Id,
                                FirstName = a.Patient.User.FirstName,
                                LastName = a.Patient.User.LastName,
                                Phone = a.Patient.User.PhoneNumber
                            },
                            Service = new ServiceResponse
                            {
                                Id = a.Service.Id,
                                Name = a.Service.Name,
                                Description = a.Service.Description,
                                DurationMinutes = a.Service.DurationMinutes
                            }
                        })
                        .ToListAsync();

                    return Ok(appointments);
                }

                return Ok(new List<AppointmentResponse>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                var user = await _userManager.Users
                    .Include(u => u.Patient)
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }
                if (user.Patient == null)
                {
                    return Forbid("Only patients can book appointments");
                }

                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = user.Patient.Id,
                    ServiceId = request.ServiceId,
                    AppointmentDate = request.AppointmentDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = "pending",
                    BookingType = "online",
                    Notes = request.Notes
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}