using Auction.Domain.Entities;

namespace Auction.Application.Common.Abstractions.Repository;

public interface ILotRepository : IBaseRepository<Lot>
{
    Task<IList<Lot>> GetCurrentLots();
}
