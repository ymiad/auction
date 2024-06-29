using Auction.Application.UserBets.Commands.CreateBet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auction.WebApi.Features;

public static class UserBets
{
    public static WebApplication UserBetsFeature(this WebApplication builder)
    {
        var group = builder
            .MapGroup($"/api/{nameof(UserBets)}");

        group.MapPost(
            "/createBet",
            async ([FromBody] CreateBetCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            await mediator.Send(request, cancellationToken))
            .WithTags(nameof(UserBets))
            .WithSummary("Place bet")
            .WithOpenApi()
            .Produces<Guid>();

        return builder;
    }
}
