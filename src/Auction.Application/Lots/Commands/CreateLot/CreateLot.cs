using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Application.Scheduling;
using Auction.Domain.Entities;
using Quartz;

namespace Auction.Application.Lots.Commands.CreateLot;

[Authorize(Role.User)]
public record CreateLotCommand : IRequest<Result<Guid>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal StartPrice { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class CreateLotCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider, IScheduler scheduler)
    : IRequestHandler<CreateLotCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IScheduler _scheduler = scheduler;
    private readonly UserProvider _userProvider = userProvider;

    public async Task<Result<Guid>> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            return userIdResult;
        }

        var userId = userIdResult.Value;

        var entity = new Lot
        {
            Name = request.Name,
            Description = request.Description,
            StartPrice = request.StartPrice,
            TradingStartDate = request.StartDate,
            TradingEndDate = request.EndDate,
            PublisherId = userId,
            OwnerId = userId,
        };

        using var connection = _unitOfWork.Create();

        var lotId = await connection.Repositories.LotRepository.Create(entity);

        await connection.SaveChangesAsync();

        await _scheduler.ActivateTradingEndJob(lotId, request.EndDate, cancellationToken);

        return Result<Guid>.Success(lotId);
    }
}
