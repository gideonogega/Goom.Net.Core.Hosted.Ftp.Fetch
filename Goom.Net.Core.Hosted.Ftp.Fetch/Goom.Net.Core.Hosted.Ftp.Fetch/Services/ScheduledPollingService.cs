using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Goom.Net.Core.Hosted.Ftp.Fetch.Services
{
    public class ScheduledPollingService : IHostedService, IDisposable
    {
        private readonly ILogger<ScheduledPollingService> logger;
        private readonly IFetchService fetchService;
        private Timer timer;

        public ScheduledPollingService(ILogger<ScheduledPollingService> logger, IFetchService fetchService)
        {
            this.logger = logger;
            this.fetchService = fetchService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Scheduled Polling Service running.");

            // Format hh:mm:ss (e.g.  00:05:00)
            var intervalString = Environment.GetEnvironmentVariable("FTP_POLL_INTERVAL");
            timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.Parse(intervalString));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            logger.LogInformation("Scheduled Polling Service is working. Count: {Count}");

            fetchService.Fetch();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Scheduled Polling Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
