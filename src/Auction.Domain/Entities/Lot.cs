using Auction.Domain.Common;

namespace Auction.Domain.Entities;

public class Lot : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal StartPrice { get; set; }

    public string Tags { get; set; }

    public DateTime TradingStart { get; set; }

    public int TradingDuration { get; set; }
}
