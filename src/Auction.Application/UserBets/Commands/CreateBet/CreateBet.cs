using Auction.Application.Common;
using Auction.Application.Common.Abstractions.Repository;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Utils;
using Auction.Domain.Entities;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Principal;

namespace Auction.Application.UserBets.Commands.CreateBet;

[Authorize]
public record CreateBetCommand : IRequest<Guid>
{
    public Guid LotId { get; init; }
    public decimal Amount { get; set; }
}

public class CreateBetCommandHandler : IRequestHandler<CreateBetCommand, Guid>
{
    private readonly UserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBetCommandHandler(UserProvider userProvider, IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateBetCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _userProvider.GetCurrentUserId();
        using var connection = _unitOfWork.Create();

        var userRepository = connection.Repositories.UserRepository;
        var userBetRepository = connection.Repositories.UserBetRepository;
        var accountRepository = connection.Repositories.AccountRepository;
        var lotRepository = connection.Repositories.LotRepository;

        Guid result = Guid.Empty;
        var lot = await lotRepository.GetById(command.LotId);
        var user = await userRepository.GetById(currentUserId);
        var account = await accountRepository.GetById(user.AccountId);

        if (lot is not null && (lot.Archived || lot.StartPrice > account.Ammount) && lot.TradingStartDate <= DateTime.Now)
        {
            return result;
        }

        var bets = await userBetRepository.GetBy(nameof(UserBet.LotId), command.LotId);
        var lastBet = bets.OrderBy(x => x.Ammount).LastOrDefault();

        if (lastBet is not null && account.Ammount > lastBet.Ammount)
        {
            var bet = new UserBet
            {
                LotId = command.LotId,
                UserId = currentUserId,
                Ammount = command.Amount,
            };

            var prevUser = await userRepository.GetById(lastBet.UserId);
            var prevAccount = await accountRepository.GetById(prevUser.AccountId);
            prevAccount.Ammount += lastBet.Ammount;
            await accountRepository.Update(prevAccount);

            result = await userBetRepository.Create(bet);
            account.Ammount -= bet.Ammount;
            await accountRepository.Update(account);

            connection.SaveChanges();
        }

        return result;
    }
}
