using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Auction.Application.Scheduling.Jobs;

internal class TriggerLotTradingEndJob(IUnitOfWork unitOfWork, ILogger<TriggerLotTradingEndJob> logger) : IJob
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<TriggerLotTradingEndJob> _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var lotId = context.Trigger.JobDataMap.Get(JobDataFieldNames.Lot.Id)?.ToString();
        if (lotId is null)
        {
            return;
        }

        var lotIdParsed = new Guid(lotId);
        using var connection = _unitOfWork.Create();
        var lot = await connection.Repositories.LotRepository.GetById(lotIdParsed);

        if (lot is null)
        {
            _logger.LogWarning("{Message}", LotError.NotFound);
            return;
        }

        var userBets = await connection.Repositories.UserBetRepository.GetBy(nameof(UserBet.LotId), lot.Id);
        if (userBets.Any())
        {
            var lastUserBet = userBets.OrderBy(x => x.Ammount).Last();
            lot.OwnerId = lastUserBet.UserId;
        }
        else
        {
            lot.Archived = true;
        }

        await connection.Repositories.LotRepository.Update(lot);
        await connection.SaveChangesAsync();
    }
}
