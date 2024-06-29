using Auction.Domain.Entities;

namespace Auction.Application.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public Roles Role { get; } = Roles.User;

        public AuthorizeAttribute(Roles role) => Role = role;

        public AuthorizeAttribute() => Role = Roles.User;
    }
}
