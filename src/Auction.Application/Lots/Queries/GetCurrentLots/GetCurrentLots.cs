using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Lots.Queries.GetLots;

public record GetCurrentLotsQuery : IRequest<Result<List<CurrentLotDto>>>;

public class GetCurrentLotsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCurrentLotsQueryHandler> logger)
    : IRequestHandler<GetCurrentLotsQuery, Result<List<CurrentLotDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetCurrentLotsQueryHandler> _logger = logger;

    public async Task<Result<List<CurrentLotDto>>> Handle(GetCurrentLotsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var lots = await connection.Repositories.LotRepository.GetCurrentLots();
        List<CurrentLotDto> result = new(lots.Count);

        foreach(var lot in lots)
        {
            var userBet = await connection.Repositories.UserBetRepository.GetBy(nameof(UserBet.LotId), lot.Id);
            var lastBet = userBet.OrderBy(x => x.Ammount).LastOrDefault();
            string lastBetUsername = string.Empty;
            decimal lastBetAmmount = default;
            if (lastBet is not null)
            {
                var user = await connection.Repositories.UserRepository.GetById(lastBet.UserId);
                if (user is null)
                {
                    _logger.LogWarning("{Message}", UserError.NotFound.Description);
                    return Result<List<CurrentLotDto>>.Failure(UserError.NotFound);
                }
                lastBetUsername = user.Username;
                lastBetAmmount = lastBet.Ammount;
            }

            result.Add(new CurrentLotDto
            {
                Id = lot.Id,
                Name = lot.Name,
                Description = lot.Description,
                TradingStartDate = lot.TradingStartDate,
                TrandingEndDate = lot.TradingEndDate,
                CurrentBetAmount = lastBetAmmount,
                CurrentUser = lastBetUsername
            });
        }

        return Result<List<CurrentLotDto>>.Success(result);
    }
}
