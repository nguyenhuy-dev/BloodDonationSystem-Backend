using Application.Service.Events;
using Application.Service.VolunteerServ;

namespace BloodDonationSystem.BackgroundServices
{
    public class VolunteerExpiryBackgroundService(IServiceProvider _serviceProvider,
                                              ILogger<EventExpiryBackgroundService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var volunteerService = scope.ServiceProvider.GetRequiredService<IVolunteerService>();

                    var expiredCount = await volunteerService.VolunteerEndDateExpiredAsync();
                    _logger.LogInformation($"[{DateTime.Now}] Marked {expiredCount} events as expired.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while expiring events.");
                }

                // Run daily (or change to run hourly: TimeSpan.FromHours(1))
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
