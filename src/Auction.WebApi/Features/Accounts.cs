using Auction.Application.Accounts.Commands.Refill;
using Auction.Application.Common.Models;
using Auction.Application.Accounts.Queries.Ammount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auction.WebApi.Features;

public static class Accounts
{
    public static WebApplication AccountsFeature(this WebApplication builder)
    {
        var group = builder
            .MapGroup($"/api/{nameof(Accounts)}");

        group.MapPost(
            "/refill",
            async ([FromBody] RefillCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            await mediator.Send(request, cancellationToken))
            .WithTags(nameof(Accounts))
            .WithSummary("Refill account")
            .WithOpenApi()
            .Produces<Result<Guid>>();

        group.MapGet(
            "/",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            await mediator.Send(new AmmountQuery(), cancellationToken))
            .WithTags(nameof(Accounts))
            .WithSummary("Account amount")
            .WithOpenApi()
            .Produces<Result<decimal>>();

        return builder;
    }
}
