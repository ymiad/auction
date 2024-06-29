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
            object? userId = null;
            var isSuccess = _httpContextAccessor?.HttpContext?.Items.TryGetValue("user_id", out userId) ?? false;
            var currentUserId = isSuccess && userId is not null ? (Guid)userId : Guid.Empty;

            return result;
        }
    }
}
