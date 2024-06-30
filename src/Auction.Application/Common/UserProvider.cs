using Auction.Application.Common.Models;
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

    public Result<Guid> GetCurrentUserId()
    {
        object? userId = null;
        var isSuccess = _httpContextAccessor?.HttpContext?.Items.TryGetValue(HttpContextConstants.UserId, out userId) ?? false;

        if (isSuccess && userId is not null)
        {
            return Result<Guid>.Success((Guid)userId);
        }

        return Result<Guid>.Failure(AuthError.Unauthorized);
    }
}
