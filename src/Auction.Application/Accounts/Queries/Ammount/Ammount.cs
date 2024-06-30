using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;

namespace Auction.Application.Accounts.Queries.Ammount;

[Authorize]
public record AmmountQuery : IRequest<decimal>;

public class AmmountQueryHandler : IRequestHandler<AmmountQuery, decimal>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserProvider _userProvider;

    public AmmountQueryHandler(IUnitOfWork unitOfWork, UserProvider userProvider)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
    }

    public async Task<decimal> Handle(AmmountQuery request, CancellationToken cancellationToken)
    {
        var userId = _userProvider.GetCurrentUserId();

        decimal amount = 0;

        using (var connection = _unitOfWork.Create())
        {
            var user = await connection.Repositories.UserRepository.GetById(userId);
            var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);
            amount = account.Ammount;
        }

        return amount;
    }
}
