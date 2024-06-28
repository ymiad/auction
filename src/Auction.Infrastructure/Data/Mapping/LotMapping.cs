using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Constants;

namespace Auction.Infrastructure.Data.Mapping;

public class LotMapping : IMapping<Lot>
{
    public Dictionary<string, string> GetMapping()
    {
        return new Dictionary<string, string>
        {
            { EntityConstants.TableName, "lots" },
            { nameof(Lot.Id), "id" },
            { nameof(Lot.Name), "name" },
            { nameof(Lot.Description), "description" },
            { nameof(Lot.StartPrice), "start_price" },
            { nameof(Lot.TradingStart), "trading_start" },
            { nameof(Lot.TradingDuration), "trading_duration" },
        };
    }
}
