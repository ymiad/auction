using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;

namespace Auction.Application.Accounts.Commands.Refill;

[Authorize]
public record RefillCommand : IRequest<Result>
{
    public decimal Amount { get; init; }
}

public class RefillCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider) : IRequestHandler<RefillCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserProvider _userProvider = userProvider;

    public async Task<Result> Handle(RefillCommand command, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            return Result.Failure(userIdResult.Error);
        }

        var userId = userIdResult.Value;

        using (var connection = _unitOfWork.Create())
        {
            var user = await connection.Repositories.UserRepository.GetById(userId);

            if (user is null)
            {
                return Result.Failure(UserError.NotFound);
            }

            var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);

            if (account is null)
            {
                return Result.Failure(AccountError.NotFound);
            }

            account.Ammount += command.Amount;
            await connection.Repositories.AccountRepository.Update(account);

            await connection.SaveChangesAsync();
        }

        return Result.Success();
    }
}
