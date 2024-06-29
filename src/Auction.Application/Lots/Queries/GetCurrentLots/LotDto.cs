using Auction.Domain.Entities;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public record CurrentLotDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public DateTime TradingStartDate { get; init; }
    public DateTime TrandingEndDate { get; init; }
    public required string CurrentUser { get; init; }
    public decimal CurrentBetAmount { get; init; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Lot,  CurrentLotDto>();
        }
    }
}
