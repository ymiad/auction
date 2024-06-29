using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Constants;

namespace Auction.Infrastructure.Data.Mapping;

public class UserMapping : IMapping<User>
{
    public Dictionary<string, string> GetMapping()
    {
        return new Dictionary<string, string>
        {
            { EntityConstants.TableName, "users" },
            { nameof(User.Id), "id" },
            { nameof(User.Username), "username" },
            { nameof(User.Password), "password" },
            { nameof(User.AccountId), "account_id" },
            { nameof(User.Role), "role" },
        };
    }
}
