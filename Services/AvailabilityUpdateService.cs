using HopewellClinicApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HopewellClinicApi.Services
{
    public class AvailabilityUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AvailabilityUpdateService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);

        public AvailabilityUpdateService(IServiceProvider serviceProvider, ILogger<AvailabilityUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Availability Update Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateAvailabilityDataAsync();
                    await Task.Delay(_updateInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Availability Update Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Availability Update Service");
                    // Continue running even if there's an error
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("Availability Update Service stopped");
        }

        private async Task UpdateAvailabilityDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var availabilityService = scope.ServiceProvider.GetRequiredService<IDoctorAvailabilityService>();
            var context = scope.ServiceProvider.GetRequiredService<HopewellDbContext>();

            try
            {
                var today = DateTime.Today;
                var endDate = today.AddDays(7); // Update next 7 days

                _logger.LogInformation("Updating availability data from {StartDate} to {EndDate}", today, endDate);

                // Get all active doctors
                var activeDoctors = await context.Staff
                    .Where(s => s.IsActive)
                    .Select(s => s.Id)
                    .ToListAsync();

                var totalUpdates = 0;
                var tasks = new List<Task>();

                // Update availability for each doctor for the next 7 days
                for (var date = today; date <= endDate; date = date.AddDays(1))
                {
                    foreach (var doctorId in activeDoctors)
                    {
                        tasks.Add(UpdateDoctorAvailabilityForDateAsync(doctorId, date));
                        totalUpdates++;
                    }
                }

                await Task.WhenAll(tasks);

                _logger.LogInformation("Successfully updated availability for {TotalUpdates} doctor-date combinations", totalUpdates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability data");
            }
        }

        private async Task UpdateDoctorAvailabilityForDateAsync(Guid doctorId, DateTime date)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var availabilityService = scope.ServiceProvider.GetRequiredService<IDoctorAvailabilityService>();
                await availabilityService.UpdateDoctorAvailabilityAsync(doctorId, date);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update availability for doctor {DoctorId} on date {Date}", doctorId, date);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Availability Update Service");
            await base.StopAsync(cancellationToken);
        }
    }
}
