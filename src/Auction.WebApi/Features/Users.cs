using Auction.Application.Common.Models;
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
            .Produces<Result<Guid>>();

        group.MapPost(
            "/login",
            async ([FromBody] LoginCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Login")
            .WithOpenApi()
            .Produces<Result<Guid>>();

        group.MapPut(
            "/ban/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new BanUserCommand(id), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Ban user")
            .WithOpenApi()
            .Produces<Result>();

        group.MapPut(
            "/setModeratorRole",
            async ([FromBody] SetModeratorRole request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Make as moderator")
            .WithOpenApi()
            .Produces<Result>();

        group.MapPut(
            "/giveAdmin/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new UpdateRoleCommand(id, Role.Admin), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Set Admin role (for testing, auth not required)")
            .WithOpenApi()
            .Produces<Result<Guid>>();

        group.MapPut(
            "/giveModer/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new UpdateRoleCommand(id, Role.Moderator), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Set Moder role(for testing, auth not required)")
            .WithOpenApi()
            .Produces<Result<Guid>>();

        group.MapPut(
           "/giveUser/{id:guid}",
           async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
           (await mediator.Send(new UpdateRoleCommand(id, Role.User), cancellationToken))
           )
           .WithTags(nameof(Users))
           .WithSummary("Set User role (for testing, auth not required)")
           .WithOpenApi()
           .Produces<Result<Guid>>();

        group.MapGet(
            "/",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new GetUsersQuery(), cancellationToken))
            )
            .WithTags(nameof(Users))
            .WithSummary("Get all users (for testing, auth not required)")
            .WithOpenApi()
            .Produces<Result<List<User>>>();

        return builder;
    }
}
