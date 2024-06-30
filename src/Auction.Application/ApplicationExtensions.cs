using Auction.Application.Common;
using Auction.Application.Common.Behaviours;
using Auction.Application.Common.Options;
using Auction.Application.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auction.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Section));

        services.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Transient;
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScheduling(connectionString);
        services.AddScoped<UserProvider>();

        return services;
    }
}
