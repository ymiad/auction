using Microsoft.Extensions.Hosting;
using Quartz;

namespace Auction.Application.Scheduling
{
    internal class SchedulingHostedService : IHostedService
    {
        private readonly IScheduler _scheduler;

        public SchedulingHostedService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduler.ScheduleTasksAndStartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(true, cancellationToken);
        }
    }
}
