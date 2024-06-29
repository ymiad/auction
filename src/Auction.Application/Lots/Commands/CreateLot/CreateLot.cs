using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Scheduling;
using Auction.Application.Utils;
using Auction.Domain.Entities;
using Quartz;

namespace Auction.Application.Lots.Commands.CreateLot;

[Authorize]
public record CreateLotCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal StartPrice { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduler _scheduler;
    private readonly UserProvider _userProvider;

    public CreateLotCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider, IScheduler scheduler)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
        _scheduler = scheduler;
    }

    public async Task<Guid> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userProvider.GetCurrentUserId();
        var entity = new Lot
        {
            Name = request.Name,
            Description = request.Description,
            StartPrice = request.StartPrice,
            TradingStartDate = request.StartDate,
            TradingEndDate = request.EndDate,
            PublisherId = currentUserId,
            OwnerId = currentUserId,
        };

        using var connection = _unitOfWork.Create();

        var lotId = await connection.Repositories.LotRepository.Create(entity);

        connection.SaveChanges();

        await _scheduler.ActivateTradingEndJob(lotId, request.EndDate);

        return lotId;
    }
}
