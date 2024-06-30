using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Accounts.Commands.Refill;

[Authorize]
public record RefillCommand : IRequest<Result>
{
    public decimal Amount { get; init; }
}

public class RefillCommandHandler(IUnitOfWork unitOfWork, UserProvider userProvider, ILogger<RefillCommandHandler> logger)
    : IRequestHandler<RefillCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserProvider _userProvider = userProvider;
    private readonly ILogger<RefillCommandHandler> _logger = logger;

    public async Task<Result> Handle(RefillCommand command, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            _logger.LogWarning("{Message}", userIdResult.Error.Description);
            return Result.Failure(userIdResult.Error);
        }

        var userId = userIdResult.Value;

        using (var connection = _unitOfWork.Create())
        {
            var user = await connection.Repositories.UserRepository.GetById(userId);

            if (user is null)
            {
                _logger.LogWarning("{Message}", UserError.NotFound.Description);
                return Result.Failure(UserError.NotFound);
            }

            var account = await connection.Repositories.AccountRepository.GetById(user.AccountId);

            if (account is null)
            {
                _logger.LogWarning("{Message}", AccountError.NotFound.Description);
                return Result.Failure(AccountError.NotFound);
            }

            account.Ammount += command.Amount;
            await connection.Repositories.AccountRepository.Update(account);

            await connection.SaveChangesAsync();
        }

        return Result.Success();
    }
}
