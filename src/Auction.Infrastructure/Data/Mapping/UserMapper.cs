using Auction.Domain.Entities;

namespace Auction.Infrastructure.Data.Mapping;

public class UserMapper : BaseMapper<User>
{
    public UserMapper() : base(
        tableName: "users",
        mapping: new Dictionary<string, string>
        {
            { nameof(User.Id), "id" },
            { nameof(User.Username), "username" },
            { nameof(User.Password), "password" },
            { nameof(User.AccountId), "account_id" },
            { nameof(User.Role), "role" },
            { nameof(User.Banned), "banned" },
        }
    ) { }
}
