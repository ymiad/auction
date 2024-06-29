using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Constants;

namespace Auction.Infrastructure.Data.Mapping;

public class AccountMapping : IMapping<Account>
{
    public Dictionary<string, string> GetMapping()
    {
        return new Dictionary<string, string>
        {
            { EntityConstants.TableName, "accounts" },
            { nameof(Account.Id), "id" },
            { nameof(Account.Ammount), "ammount" },
        };
    }
}
