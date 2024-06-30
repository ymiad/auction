using Auction.Domain.Entities;

namespace Auction.Infrastructure.Data.Mapping;

public class UserBetMapper : BaseMapper<UserBet>
{
    public UserBetMapper() : base(
        tableName: "user_bets",
        mapping: new Dictionary<string, string>
        {
            { nameof(UserBet.Id), "id" },
            { nameof(UserBet.UserId), "user_id" },
            { nameof(UserBet.LotId), "lot_id" },
            { nameof(UserBet.Ammount), "ammount" },
        }
    ) { }
}
