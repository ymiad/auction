using Auction.Application.Lots.Commands.CreateLot;
using Auction.Application.Lots.Queries.GetLots;
using Auction.Application.Users.Commands.Login;
using Auction.Application.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auction.WebApi.Features;

public static class Users
{
    //[Authorize]
    public static WebApplication UsersFeature(this WebApplication builder)
    {
        var group = builder
            .MapGroup($"/api/{nameof(Users)}");

        group.MapPost(
            "/register",
            async ([FromBody] RegisterCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Register")
            .WithOpenApi()
            .Produces<Guid>();

        group.MapPost(
            "/login",
            async ([FromBody] LoginCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Login")
            .WithOpenApi()
            .Produces<Guid>();

        return builder;
    }
}
