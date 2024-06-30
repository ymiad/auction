using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;

namespace Auction.Application.Accounts.Commands.Refill;

[Authorize]
public record RefillCommand : IRequest
{
    public decimal Amount { get; init; }
}

public class RefillCommandHandler : IRequestHandler<RefillCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserProvider _userProvider;

    public RefillCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
    }

    public async Task Handle(RefillCommand command, CancellationToken cancellationToken)
    {
        var userId = _userProvider.GetCurrentUserId();

        using (var connection = _unitOfWork.Create())
        {
            var user = await connection.Repositories.UserRepository.GetById(userId);
            var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);
            account.Ammount += command.Amount;
            await connection.Repositories.AccountRepository.Update(account);

            connection.SaveChanges();
        }
    }
}
