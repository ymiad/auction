using Auction.Application.Common.Models;
using Auction.Application.Lots.Commands.CreateLot;
using Auction.Application.Lots.Commands.DeleteLot;
using Auction.Application.Lots.Queries.GetLots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auction.WebApi.Features;

public static class Lots
{
    public static WebApplication LotsFeature(this WebApplication builder)
    {
        var group = builder
            .MapGroup($"/api/{nameof(Lots)}");            

        group.MapPost(
            "/create",
            async ([FromBody] CreateLotCommand request, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(request, cancellationToken))
            )
            .WithTags(nameof(Lots))
            .WithSummary("Create lot")
            .WithOpenApi()
            .Produces<Result<Guid>>();
        
        group.MapGet(
            "/getAll",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new GetLotsQuery(), cancellationToken))
            )
            .WithTags(nameof(Lots))
            .WithSummary("Get all lots")
            .WithOpenApi()
            .Produces<Result<List<LotDto>>>();

        group.MapGet(
            "/getCurrentLots",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new GetCurrentLotsQuery(), cancellationToken))
            )
            .WithTags(nameof(Lots))
            .WithSummary("Get current available lots")
            .WithOpenApi()
            .Produces<Result<List<CurrentLotDto>>>();

        group.MapDelete(
            "/{id:guid}",
            async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            (await mediator.Send(new DeleteLotCommand(id), cancellationToken))
            )
            .WithTags(nameof(Lots))
            .WithSummary("Delete lot")
            .WithOpenApi()
            .Produces<Result>();

        return builder;
    }
}
