using System.Diagnostics.CodeAnalysis;

namespace Auction.WebApi.Infrastructure;

public static class EndpointRouteBuilderExtension
{
    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder endpointRouteBuilder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        endpointRouteBuilder.MapGet(pattern, handler)
            .WithName(handler.Method.Name);

        return endpointRouteBuilder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder endpointRouteBuilder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        endpointRouteBuilder.MapPost(pattern, handler)
            .WithName(handler.Method.Name);

        return endpointRouteBuilder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder endpointRouteBuilder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        endpointRouteBuilder.MapPut(pattern, handler)
            .WithName(handler.Method.Name);

        return endpointRouteBuilder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder endpointRouteBuilder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        endpointRouteBuilder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name);

        return endpointRouteBuilder;
    }
}
