using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Constants;

namespace Auction.Infrastructure.Data.Mapping
{
    public class UserBetMapping : IMapping<UserBet>
    {
        public Dictionary<string, string> GetMapping()
        {
            return new Dictionary<string, string> {
                { EntityConstants.TableName, "user_bets" },
                { nameof(UserBet.Id), "id" },
                { nameof(UserBet.UserId), "user_id" },
                { nameof(UserBet.LotId), "lot_id" },
                { nameof(UserBet.Ammount), "ammount" },
            };
        }
    }
}
