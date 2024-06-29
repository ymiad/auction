using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;
using Quartz;

namespace Auction.Application.Scheduling.Jobs;

internal class TriggerLotTradingEndJob : IJob
{
    private readonly IUnitOfWork _unitOfWork;

    public TriggerLotTradingEndJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var lotId = new Guid(context.Trigger.JobDataMap.Get(JobDataFieldNames.Lot.Id).ToString());
        using var connection = _unitOfWork.Create();
        var lot = await connection.Repositories.LotRepository.GetById(lotId);
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
        connection.SaveChanges();
    }
}
