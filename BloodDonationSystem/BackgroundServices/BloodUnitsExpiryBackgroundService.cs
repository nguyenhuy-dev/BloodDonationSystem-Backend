using Application.Service.BloodInventoryServ;

namespace BloodDonationSystem.BackgroundServices
{
    public class BloodUnitsExpiryBackgroundService(IServiceProvider _serviceProvider, 
                                                    ILogger<BloodUnitsExpiryBackgroundService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var bloodInventoryService = scope.ServiceProvider.GetRequiredService<IBloodInventoryService>();

                    var expiredCount = await bloodInventoryService.GetBloodUnitsExpiredAsync();
                    _logger.LogInformation($"[{DateTime.Now}] Marked {expiredCount} blood units as expired.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while expiring.");
                }

                // Run daily (or change to run hourly: TimeSpan.FromHours(1))
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
