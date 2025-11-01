using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;
using HopewellClinicApi.Attributes;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeAdmin]
    public class ReportsController : ControllerBase
    {
        private readonly HopewellDbContext _context;

        public ReportsController(HopewellDbContext context)
        {
            _context = context;
        }

        [HttpGet("appointment-statistics")]
        public async Task<ActionResult<AppointmentStatisticsResponse>> GetAppointmentStatistics(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                var appointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var totalAppointments = appointments.Count;
                var completedAppointments = appointments.Count(a => a.Status == "completed");
                var cancelledAppointments = appointments.Count(a => a.Status == "cancelled");
                var pendingAppointments = appointments.Count(a => a.Status == "pending");
                var walkInAppointments = appointments.Count(a => a.IsWalkIn);

                var completionRate = totalAppointments > 0 ? (double)completedAppointments / totalAppointments * 100 : 0;
                var cancellationRate = totalAppointments > 0 ? (double)cancelledAppointments / totalAppointments * 100 : 0;

                // Daily statistics
                var dailyStats = appointments
                    .GroupBy(a => a.AppointmentDate)
                    .Select(g => new DailyAppointmentStats
                    {
                        Date = g.Key,
                        TotalAppointments = g.Count(),
                        CompletedAppointments = g.Count(a => a.Status == "completed"),
                        CancelledAppointments = g.Count(a => a.Status == "cancelled"),
                        WalkInAppointments = g.Count(a => a.IsWalkIn)
                    })
                    .OrderBy(s => s.Date)
                    .ToList();

                var response = new AppointmentStatisticsResponse
                {
                    TotalAppointments = totalAppointments,
                    CompletedAppointments = completedAppointments,
                    CancelledAppointments = cancelledAppointments,
                    PendingAppointments = pendingAppointments,
                    WalkInAppointments = walkInAppointments,
                    CompletionRate = Math.Round(completionRate, 2),
                    CancellationRate = Math.Round(cancellationRate, 2),
                    DailyStats = dailyStats
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("service-usage")]
        public async Task<ActionResult<ServiceUsageResponse>> GetServiceUsage(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var totalAppointments = appointments.Count;

                var serviceStats = appointments
                    .GroupBy(a => a.ServiceId)
                    .Select(g => new ServiceUsageStats
                    {
                        ServiceId = g.Key,
                        ServiceName = g.First().Service.Name,
                        UsageCount = g.Count(),
                        TotalRevenue = g.Sum(a => a.ServicePrice ?? 0),
                        UsagePercentage = totalAppointments > 0 ? (double)g.Count() / totalAppointments * 100 : 0
                    })
                    .OrderByDescending(s => s.UsageCount)
                    .ToList();

                var mostPopularService = serviceStats.FirstOrDefault()?.ServiceName ?? "N/A";
                var leastPopularService = serviceStats.LastOrDefault()?.ServiceName ?? "N/A";

                var response = new ServiceUsageResponse
                {
                    ServiceStats = serviceStats,
                    TotalServicesUsed = serviceStats.Count,
                    MostPopularService = mostPopularService,
                    LeastPopularService = leastPopularService
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReportResponse>> GetRevenueReport(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Staff)
                        .ThenInclude(s => s.User)
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    .ToListAsync();

                var totalRevenue = appointments.Sum(a => a.ServicePrice ?? 0);
                var pendingRevenue = appointments
                    .Where(a => a.PaymentStatus == "pending")
                    .Sum(a => a.ServicePrice ?? 0);
                var refundedRevenue = appointments
                    .Where(a => a.PaymentStatus == "refunded")
                    .Sum(a => a.ServicePrice ?? 0);

                // Daily revenue
                var dailyRevenue = appointments
                    .GroupBy(a => a.AppointmentDate)
                    .Select(g => new DailyRevenueStats
                    {
                        Date = g.Key,
                        Revenue = g.Sum(a => a.ServicePrice ?? 0),
                        AppointmentCount = g.Count()
                    })
                    .OrderBy(r => r.Date)
                    .ToList();

                // Service revenue
                var serviceRevenue = appointments
                    .GroupBy(a => a.ServiceId)
                    .Select(g => new ServiceRevenueStats
                    {
                        ServiceId = g.Key,
                        ServiceName = g.First().Service.Name,
                        Revenue = g.Sum(a => a.ServicePrice ?? 0),
                        AppointmentCount = g.Count(),
                        RevenuePercentage = totalRevenue > 0 ? (double)(g.Sum(a => a.ServicePrice ?? 0) / totalRevenue * 100) : 0
                    })
                    .OrderByDescending(s => s.Revenue)
                    .ToList();

                // Doctor revenue
                var doctorRevenue = appointments
                    .Where(a => a.StaffId.HasValue)
                    .GroupBy(a => a.StaffId)
                    .Select(g => new DoctorRevenueStats
                    {
                        DoctorId = g.Key!.Value,
                        DoctorName = $"{g.First().Staff!.User.FirstName} {g.First().Staff.User.LastName}",
                        Revenue = g.Sum(a => a.ServicePrice ?? 0),
                        AppointmentCount = g.Count(),
                        RevenuePercentage = totalRevenue > 0 ? (double)(g.Sum(a => a.ServicePrice ?? 0) / totalRevenue * 100) : 0
                    })
                    .OrderByDescending(d => d.Revenue)
                    .ToList();

                var response = new RevenueReportResponse
                {
                    TotalRevenue = totalRevenue,
                    PendingRevenue = pendingRevenue,
                    RefundedRevenue = refundedRevenue,
                    DailyRevenue = dailyRevenue,
                    ServiceRevenue = serviceRevenue,
                    DoctorRevenue = doctorRevenue
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<AdminDashboardSummary>> GetDashboardSummary()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalPatients = await _context.Patients.CountAsync();
                var totalDoctors = await _context.Staff
                    .Include(s => s.User)
                    .CountAsync(s => s.User.IsActive);
                var totalServices = await _context.Services.CountAsync();
                var activeServices = await _context.Services.CountAsync(s => s.IsActive);

                var today = DateTime.Today;
                var totalAppointmentsToday = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate == today);
                var todayRevenue = await _context.Appointments
                    .Where(a => a.AppointmentDate == today)
                    .SumAsync(a => a.ServicePrice ?? 0);

                var recentServices = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(5)
                    .Select(s => new ServiceResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
                        Price = s.Price,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt
                    })
                    .ToListAsync();

                var response = new AdminDashboardSummary
                {
                    TotalUsers = totalUsers,
                    TotalPatients = totalPatients,
                    TotalDoctors = totalDoctors,
                    TotalNurses = 0,
                    TotalServices = totalServices,
                    ActiveServices = activeServices,
                    TotalAppointmentsToday = totalAppointmentsToday,
                    TodayRevenue = todayRevenue,
                    RecentServices = recentServices
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
