using Auction.Domain.Common;

namespace Auction.Domain.Entities;

public class Lot : BaseEntity
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public decimal StartPrice { get; set; }

    public DateTime TradingStartDate { get; set; }

    public DateTime TradingEndDate { get; set; }

    public Guid PublisherId { get; set; }

    public Guid OwnerId { get; set; }

    public bool Archived { get; set; }
}
