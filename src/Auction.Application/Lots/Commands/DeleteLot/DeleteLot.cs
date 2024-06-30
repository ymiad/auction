using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Quartz;

namespace Auction.Application.Lots.Commands.DeleteLot;

[Authorize(Role.Moderator | Role.Admin)]
public record DeleteLotCommand(Guid LotId) : IRequest<Result>;

public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduler _scheduler;
    private readonly UserProvider _userProvider;

    public DeleteLotCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider, IScheduler scheduler)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
        _scheduler = scheduler;
    }

    public async Task<Result> Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var lot = await connection.Repositories.LotRepository.GetById(command.LotId);

        if (lot is null)
        {
            return Result.Failure(LotError.NotFound);
        }

        await connection.Repositories.LotRepository.Delete(command.LotId);

        await connection.SaveChangesAsync();

        return Result.Success();
    }
}
