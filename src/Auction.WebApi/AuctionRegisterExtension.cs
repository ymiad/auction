using Auction.Application;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Infrastructure.Data.Migrations;
using Auction.Infrastructure.Data.UnitOfWork;
using FluentMigrator.Runner;
using System.Reflection;

namespace Auction.WebApi;

public static class AuctionRegisterExtension
{
    public static IServiceCollection AddAuctionExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        using (var serviceProvider = AddAuctionData())
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider);
        }

        services.AddApplicationServices();

        services.AddUnitOfWork(configuration);

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        services.AddTransient<IUnitOfWork, UnitOfWork>(s => new UnitOfWork(connectionString));

        return services;
    }

    private static ServiceProvider AddAuctionData()
    {
        var assembly = Assembly.GetAssembly(typeof(AddLotTable));

        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString("Data Source=auction.db")
                .ScanIn(assembly))
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();
    }
}
