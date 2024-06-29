using Auction.Domain.Entities;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public class LotDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public DateTime TradingStartDate { get; set; }
    public DateTime TradingEndDate { get; set; }
    public bool Archived { get; set; }
    public Guid PublisherId { get; set; }
    public Guid OwnerId { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Lot,  LotDto>();
        }
    }
}
