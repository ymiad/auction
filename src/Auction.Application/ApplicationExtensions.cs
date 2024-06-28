using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auction.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.Lifetime = ServiceLifetime.Transient;
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
