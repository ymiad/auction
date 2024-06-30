using Auction.Application.Common.Models;
using Auction.Application.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Common;

public class UserProvider(IHttpContextAccessor httpContextAccessor, ILogger<UserProvider> logger)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<UserProvider> _logger = logger;

    public Result<Guid> GetCurrentUserId()
    {
        object? userId = null;
        var isSuccess = _httpContextAccessor?.HttpContext?.Items.TryGetValue(HttpContextConstants.UserId, out userId) ?? false;

        if (isSuccess && userId is not null)
        {
            return Result<Guid>.Success((Guid)userId);
        }

        _logger.LogWarning("{Message}", AuthError.Unauthorized.Description);
        return Result<Guid>.Failure(AuthError.Unauthorized);
    }
}
