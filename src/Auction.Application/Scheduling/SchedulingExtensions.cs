using Auction.Application.Scheduling.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Collections.Specialized;

namespace Auction.Application.Scheduling;

public static class SchedulingExtensions
{
    public static IServiceCollection AddScheduling(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<IJobFactory, JobFactory>();

        var properties = new NameValueCollection();

        services.AddSingleton(serviceProvider =>
        {
            var quartzConfig = new QuartzConfig();
            quartzConfig.SetConnectionString(connectionString);
            var quartzConfigNameValue = quartzConfig.ToNameValueCollection();
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(quartzConfigNameValue);
            var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            scheduler.JobFactory = serviceProvider.GetService<IJobFactory>();
            return scheduler;
        });

        services.AddTradingJobs();
        services.AddHostedService<SchedulingHostedService>();

        return services;
    }

    private static IServiceCollection AddTradingJobs(this IServiceCollection services)
    {
        services.AddTransient<TriggerLotTradingEndJob>();

        return services;
    }

    public static async Task ScheduleTasksAndStartAsync(this IScheduler scheduler, CancellationToken cancellationToken = default)
    {
        
        await scheduler.Start(cancellationToken);
    }

    public static async Task ActivateTradingEndJob(this IScheduler scheduler, Guid lotId, DateTime tradingEndDate, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(nameof(TriggerLotTradingEndJob));

        var job = JobBuilder
            .Create<TriggerLotTradingEndJob>()
            .StoreDurably()
            .WithIdentity(jobKey)
            .Build();

        await scheduler.AddJob(job, true, cancellationToken);

        await ScheduleActivateLotTradingEndJob(scheduler, lotId, tradingEndDate, cancellationToken);
    }

    private static async Task ScheduleActivateLotTradingEndJob(IScheduler scheduler, Guid lotId, DateTime tradingEndDate, CancellationToken cancellationToken = default)
    {
        var jobKey = new JobKey(nameof(TriggerLotTradingEndJob));

        var triggerKey = $"{nameof(TriggerLotTradingEndJob)}_{lotId}";

        JobDataMap jobData = new JobDataMap
        {
            [JobDataFieldNames.Lot.Id] = lotId,
        };

        var lotSoldTrigger = TriggerBuilder
            .Create()
            .WithIdentity(triggerKey)
            .ForJob(jobKey)
            .StartAt(tradingEndDate)
            .UsingJobData(jobData)
            .Build();

        await scheduler.ScheduleJob(lotSoldTrigger, cancellationToken);
    }
}