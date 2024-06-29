using System.Reflection;

namespace Auction.WebApi.Infrastructure;

public static class WebAppExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase endpointGroup)
    {
        var groupName = endpointGroup.GetType().Name;

        return app
            .MapGroup($"/api/{groupName}")
            .WithGroupName(groupName)
            .WithTags(groupName)
            .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly
            .GetExportedTypes()
            .Where(x => x.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}
