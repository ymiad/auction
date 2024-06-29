using Auction.Application.Lots.Commands.CreateLot;
using Auction.Application.Lots.Commands.DeleteLot;
using Auction.Application.Lots.Queries.GetLots;
using Auction.Application.Users.Commands.BanUser;
using Auction.Application.Users.Commands.GiveModeratorPermissions;
using Auction.Application.Users.Commands.Login;
using Auction.Application.Users.Commands.Register;
using Auction.Application.Users.Commands.RoleUpdate;
using Auction.Application.Users.Queries.GetUsers;
using Auction.Domain.Entities;
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

        group.MapPut(
            "/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new BanUserCommand(id), cancellationToken))
            )
            .WithTags(nameof(Lots))
            .WithSummary("Ban user")
            .WithOpenApi()
            .Produces<bool>();

        group.MapPut(
            "/giveModeratorPermissions/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new GiveModeratorPermissionsCommand(id), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Give moderator permissions")
            .WithOpenApi()
            .Produces<bool>();

        group.MapPut(
            "/changeRole",
            async ([FromBody] UpdateRoleCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Only for testing: Update role")
            .WithOpenApi()
            .Produces<Guid>();

        group.MapGet(
            "/getAll",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new GetUsersQuery(), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Get all users")
            .WithOpenApi()
            .Produces<List<User>>();

        return builder;
    }
}
