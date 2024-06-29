using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Auction.Application.Utils
{
    public class UserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            Guid result = Guid.Empty;
            object? user = null;
            var isSuccess = _httpContextAccessor?.HttpContext?.Items.TryGetValue("User", out user) ?? false;
            var currentUser = isSuccess && user is not null ? user as User : null;
            if (currentUser is not null)
            {
                result = currentUser.Id;
            }

            return result;
        }
    }
}
