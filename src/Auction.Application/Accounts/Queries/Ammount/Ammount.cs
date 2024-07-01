using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Accounts.Queries.Ammount;

[Authorize(Role.User)]
public record AmmountQuery : IRequest<Result<decimal>>;

public class AmmountQueryHandler(IUnitOfWork unitOfWork, UserProvider userProvider, ILogger<AmmountQueryHandler> logger)
    : IRequestHandler<AmmountQuery, Result<decimal>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserProvider _userProvider = userProvider;
    private readonly ILogger<AmmountQueryHandler> _logger = logger;

    public async Task<Result<decimal>> Handle(AmmountQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            _logger.LogWarning("{Message}", userIdResult.Error.Description);
            return Result<decimal>.Failure(userIdResult.Error);
        }

        var userId = userIdResult.Value;

        using var connection = _unitOfWork.Create();
        var user = await connection.Repositories.UserRepository.GetById(userId);

        if (user is null)
        {
            _logger.LogWarning("{Message}", UserError.NotFound.Description);
            return Result<decimal>.Failure(UserError.NotFound);
        }

        var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);

        if (account is null)
        {
            _logger.LogWarning("{Message}", AccountError.NotFound.Description);
            return Result<decimal>.Failure(AccountError.NotFound);
        }

        return Result<decimal>.Success(account.Ammount);
    }
}
