using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Infrastructure.Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Auction.Infrastructure;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddUnitOfWork(connectionString);

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>(s => new UnitOfWork(connectionString));

        return services;
    }
}
