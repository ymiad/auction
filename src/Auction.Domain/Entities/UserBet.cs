namespace Auction.Domain.Entities
{
    public class UserBet : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid LotId { get; set; }
        public decimal Ammount { get; set; }
    }
}
