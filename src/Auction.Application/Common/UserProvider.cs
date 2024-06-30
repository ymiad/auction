using Auction.Application.Common.Security;
using Microsoft.AspNetCore.Http;

namespace Auction.Application.Common;

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
        var isSuccess = _httpContextAccessor?.HttpContext?.Items.TryGetValue(HttpContextConstants.UserId, out userId) ?? false;
        result = isSuccess && userId is not null ? (Guid)userId : Guid.Empty;

        return result;
    }
}
