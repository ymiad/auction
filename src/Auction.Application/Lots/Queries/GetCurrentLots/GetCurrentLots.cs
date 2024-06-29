using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public record GetCurrentLotsQuery : IRequest<List<CurrentLotDto>>;

public class GetCurrentLotsQueryHandler : IRequestHandler<GetCurrentLotsQuery, List<CurrentLotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCurrentLotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CurrentLotDto>> Handle(GetCurrentLotsQuery request, CancellationToken cancellationToken)
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

        return result;
    }
}
