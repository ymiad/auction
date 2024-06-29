using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Utils;
using Auction.Domain.Entities;

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

        Guid result = Guid.Empty;
        var lot = await connection.Repositories.LotRepository.GetById(command.LotId);
        if (lot is not null && lot.Archived)
        {
            return result;
        }

        var bets = await connection.Repositories.UserBetRepository.GetBy(nameof(UserBet.LotId), command.LotId);

        if (!bets.Any() || !bets.Any(x => x.Ammount >= command.Amount))
        {
            var bet = new UserBet
            {
                LotId = command.LotId,
                UserId = currentUserId,
                Ammount = command.Amount,
            };

            result = await connection.Repositories.UserBetRepository.Create(bet);

            connection.SaveChanges();
        }

        return result;
    }
}
