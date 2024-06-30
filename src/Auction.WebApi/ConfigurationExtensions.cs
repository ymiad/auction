using Auction.Application;
using Auction.Application.Common.Options;
using Auction.Infrastructure;
using Auction.Infrastructure.Data.Migrations;
using Auction.WebApi.Infrastructure;
using FluentMigrator.Runner;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Auction.WebApi;

public static class ConfigurationExtensions
{
    private static string _connectionString = default!;

    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnectionString")!;
        CreateDatabase.CreateDataBaseIfNotExists(_connectionString);

        using (var serviceProvider = AddAuctionData(_connectionString))
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider, _connectionString);
        }

        services.AddInfrastructureServices(_connectionString);

        services.AddApplicationServices(configuration, _connectionString);

        services.AddHttpContextAccessor();
        services.AddRazorPages();

        services.AddEndpointsApiExplorer();

        services.AddSwagger();

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auction app",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Bearer {your JWT token}."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }

    private static ServiceProvider AddAuctionData(string connectionString)
    {
        var assembly = Assembly.GetAssembly(typeof(AddLotsTable));

        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(assembly))
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider, string connectionString)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();
    }
}
