using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Utils;
using Auction.Domain.Entities;
using Quartz;

namespace Auction.Application.Lots.Commands.DeleteLot;

[Authorize(Roles.Moderator)]
public record DeleteLotCommand(Guid LotId) : IRequest<bool>;

public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand, bool>
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

    public async Task<bool> Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        await connection.Repositories.LotRepository.Delete(command.LotId);

        connection.SaveChanges();

        return true;
    }
}
