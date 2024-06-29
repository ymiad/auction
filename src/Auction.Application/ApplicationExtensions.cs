using Auction.Application.Common.Behaviours;
using Auction.Application.Scheduling;
using Auction.Application.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auction.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
        {
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
}
