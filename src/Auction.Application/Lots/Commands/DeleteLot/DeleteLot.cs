using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Auction.Application.Lots.Commands.DeleteLot;

[Authorize(Role.Moderator | Role.Admin)]
public record DeleteLotCommand(Guid LotId) : IRequest<Result>;

public class DeleteLotCommandHandler(
    IUnitOfWork unitOfWork,
    IScheduler scheduler,
    ILogger<DeleteLotCommandHandler> logger)
        : IRequestHandler<DeleteLotCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IScheduler _scheduler = scheduler;
    private readonly ILogger<DeleteLotCommandHandler> _logger = logger;

    public async Task<Result> Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var lot = await connection.Repositories.LotRepository.GetById(command.LotId);

        if (lot is null)
        {
            _logger.LogWarning("{Message}", LotError.NotFound.Description);
            return Result.Failure(LotError.NotFound);
        }

        await connection.Repositories.LotRepository.Delete(command.LotId);

        await connection.SaveChangesAsync();

        //TODO: cancel scheduler

        return Result.Success();
    }
}
