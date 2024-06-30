using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;

namespace Auction.Application.Accounts.Queries.Ammount;

[Authorize]
public record AmmountQuery : IRequest<Result<decimal>>;

public class AmmountQueryHandler(IUnitOfWork unitOfWork, UserProvider userProvider) : IRequestHandler<AmmountQuery, Result<decimal>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserProvider _userProvider = userProvider;

    public async Task<Result<decimal>> Handle(AmmountQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            return Result<decimal>.Failure(userIdResult.Error);
        }

        var userId = userIdResult.Value;

        using var connection = _unitOfWork.Create();
        var user = await connection.Repositories.UserRepository.GetById(userId);

        if (user is null)
        {
            return Result<decimal>.Failure(UserError.NotFound);
        }

        var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);

        if (account is null)
        {
            return Result<decimal>.Failure(AccountError.NotFound);
        }

        return Result<decimal>.Success(account.Ammount);
    }
}
