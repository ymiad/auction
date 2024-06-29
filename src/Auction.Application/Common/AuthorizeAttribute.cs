using Auction.Domain.Entities;

namespace Auction.Application.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public Role Role { get; } = Role.User;

        public AuthorizeAttribute(Role role) => Role = role;

        public AuthorizeAttribute() => Role = Role.User;
    }
}
