using Auction.Domain.Entities;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public class LotDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Lot,  LotDto>();
        }
    }
}
