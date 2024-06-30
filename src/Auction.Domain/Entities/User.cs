using Auction.Domain.Common;

namespace Auction.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public Role Role { get; set; }

    public Guid AccountId { get; set; }

    public bool Banned { get; set; }
}
