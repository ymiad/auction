using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Application.Scheduling;
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

        var returnMoneyResult = await ReturnMoneyFromLastBet(connection.Repositories, command.LotId);

        if (returnMoneyResult.IsFailure)
        {
            _logger.LogWarning("{Message}", returnMoneyResult.Error.Description);
            return returnMoneyResult;
        }

        await connection.Repositories.LotRepository.Delete(command.LotId);

        await connection.SaveChangesAsync();

        await _scheduler.UnscheduleTradingJob(command.LotId, cancellationToken);

        return Result.Success();
    }

    private async Task<Result> ReturnMoneyFromLastBet(IUnitOfWorkRepository repositories, Guid lotId)
    {
        var bets = await repositories.UserBetRepository.GetBy(nameof(UserBet.LotId), lotId);
        var lastBet = bets.OrderBy(x => x.Ammount).LastOrDefault();

        if (lastBet is null) {
            return Result.Success();
        }

        var user = await repositories.UserRepository.GetById(lastBet.UserId);

        if (user is null)
        {
            return Result.Failure(UserError.NotFound);
        }

        var account = await repositories.AccountRepository.GetById(user.AccountId);

        if (account is null)
        {
            return Result.Failure(AccountError.NotFound);
        }

        account.Ammount += lastBet.Ammount;
        await repositories.AccountRepository.Update(account);

        return Result.Success();
    }
}
