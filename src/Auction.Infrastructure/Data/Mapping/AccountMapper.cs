using Auction.Domain.Entities;

namespace Auction.Infrastructure.Data.Mapping;

public class AccountMapper : BaseMapper<Account>
{
    public AccountMapper() : base(
        tableName: "accounts",
        mapping: new Dictionary<string, string>
        {
            { nameof(Account.Id), "id" },
            { nameof(Account.Ammount), "ammount" },
        }
    ) { }
}
