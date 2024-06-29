namespace Auction.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public Guid RoleId { get; set; }

    public Guid AccountId { get; set; }
}
