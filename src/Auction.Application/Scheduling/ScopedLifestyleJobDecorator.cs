using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Auction.Application.Scheduling;

internal class ScopedLifestyleJobDecorator : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Type _jobType;

    public ScopedLifestyleJobDecorator(IServiceProvider serviceProvider, Type jobType)
    {
        _serviceProvider = serviceProvider;
        _jobType = jobType;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            if (scope.ServiceProvider.GetService(_jobType) is IJob job)
            {
                try
                {
                    await job.Execute(context);
                }
                catch (Exception e)
                {
                    var msg = $"Unhandled exception occured while executing job '{_jobType.FullName}'";
                    var jobExecutionException = new JobExecutionException(e);
                    throw jobExecutionException;
                }
            }
        }
    }
}
