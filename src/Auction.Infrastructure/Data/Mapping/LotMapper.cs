using Auction.Domain.Entities;

namespace Auction.Infrastructure.Data.Mapping;

public class LotMapper : BaseMapper<Lot>
{
    public LotMapper() : base(
        tableName: "lots",
        mapping: new Dictionary<string, string>
        {
            { nameof(Lot.Id), "id" },
            { nameof(Lot.Name), "name" },
            { nameof(Lot.Description), "description" },
            { nameof(Lot.StartPrice), "start_price" },
            { nameof(Lot.TradingStartDate), "trading_start_date" },
            { nameof(Lot.TradingEndDate), "trading_end_date" },
            { nameof(Lot.Archived), "archived" },
            { nameof(Lot.OwnerId), "owner_id" },
            { nameof(Lot.PublisherId), "publisher_id" },
        }
    ) { }
}
