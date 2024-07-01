using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.UserBets.Commands.CreateBet;

[Authorize(Role.User)]
public record CreateBetCommand : IRequest<Result<Guid>>
{
    public Guid LotId { get; init; }
    public decimal Amount { get; set; }
}

public class CreateBetCommandHandler(
    UserProvider userProvider,
    IUnitOfWork unitOfWork,
    ILogger<CreateBetCommandHandler> logger)
        : IRequestHandler<CreateBetCommand, Result<Guid>>
{
    private readonly UserProvider _userProvider = userProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateBetCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateBetCommand command, CancellationToken cancellationToken)
    {
        var userIdResult = _userProvider.GetCurrentUserId();

        if (userIdResult.IsFailure)
        {
            _logger.LogWarning("{Message}", AuthError.Unauthorized.Description);
            return Result<Guid>.Failure(AuthError.Unauthorized);
        }

        var userId = userIdResult.Value;

        using var connection = _unitOfWork.Create();

        var userRepository = connection.Repositories.UserRepository;
        var userBetRepository = connection.Repositories.UserBetRepository;
        var accountRepository = connection.Repositories.AccountRepository;
        var lotRepository = connection.Repositories.LotRepository;

        var lot = await lotRepository.GetById(command.LotId);
        if (lot is null)
        {
            _logger.LogWarning("{Message}", LotError.NotFound.Description);
            return Result<Guid>.Failure(LotError.NotFound);
        }

        var user = await userRepository.GetById(userId);

        if (user is null)
        {
            _logger.LogWarning("{Message}", UserError.NotFound.Description);
            return Result<Guid>.Failure(UserError.NotFound);
        }

        if (lot.PublisherId == user.Id)
        {
            _logger.LogWarning("{Message}", UserBetError.SameUser.Description);
            return Result<Guid>.Failure(UserBetError.SameUser);
        }

        var account = await accountRepository.GetById(user.AccountId);

        if (account is null)
        {
            _logger.LogWarning("{Message}", AccountError.NotFound.Description);
            return Result<Guid>.Failure(AccountError.NotFound);
        }

        var bets = await userBetRepository.GetBy(nameof(UserBet.LotId), command.LotId);
        var lastBet = bets.OrderBy(x => x.Ammount).LastOrDefault();

        var validateResult = Validate(lot, account, lastBet);

        if (validateResult.IsFailure)
        {
            return validateResult;
        }
        

        if (lastBet is not null)
        {
            var returnResult = await ReturnAmmount(connection.Repositories, lastBet);
            if (returnResult.IsFailure)
            {
                _logger.LogWarning("{Message}", returnResult.Error.Description);
                return returnResult;
            }
        }

        var bet = new UserBet
        {
            LotId = command.LotId,
            UserId = userId,
            Ammount = command.Amount,
        };

        var createResult = await userBetRepository.Create(bet);
        account.Ammount -= bet.Ammount;
        await accountRepository.Update(account);

        await connection.SaveChangesAsync();

        return Result<Guid>.Success(createResult);
    }

    private async Task<Result<Guid>> ReturnAmmount(IUnitOfWorkRepository repositories, UserBet lastBet)
    {
        var prevUser = await repositories.UserRepository.GetById(lastBet.UserId);

        if (prevUser is null)
        {
            return Result<Guid>.Failure(UserError.NotFound);
        }

        var prevAccount = await repositories.AccountRepository.GetById(prevUser.AccountId);

        if (prevAccount is null)
        {
            return Result<Guid>.Failure(AccountError.NotFound);
        }

        prevAccount.Ammount += lastBet.Ammount;
        await repositories.AccountRepository.Update(prevAccount);

        return Result<Guid>.Success(Guid.Empty);
    }

    private Result<Guid> Validate(Lot lot, Account account, UserBet? lastBet)
    {
        if (lot.Archived)
        {
            return Result<Guid>.Failure(LotError.Archived);
        }

        if (lot.StartPrice > account.Ammount)
        {
            return Result<Guid>.Failure(UserBetError.AmmountLessThanPrice);
        }

        if (lot.TradingStartDate > DateTime.Now)
        {
            return Result<Guid>.Failure(LotError.TradingNotStarted);
        }

        if (lot.PublisherId != lot.OwnerId)
        {
            return Result<Guid>.Failure(LotError.Sold);
        }

        if (lastBet is not null && account.Ammount <= lastBet.Ammount)
        {
            return Result<Guid>.Failure(UserBetError.AmmountLessThanPrice);
        }

        return Result<Guid>.Success(Guid.Empty);
    }
}
