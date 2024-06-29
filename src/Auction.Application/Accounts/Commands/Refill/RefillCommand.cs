using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Auction.Application.Accounts.Commands.Refill;

[Authorize]
public record RefillCommand : IRequest
{
    public decimal Amount { get; init; }
}

public class RefillCommandHandler : IRequestHandler<RefillCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;

    public RefillCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = httpContextAccessor;
    }

    public async Task Handle(RefillCommand command, CancellationToken cancellationToken)
    {
        var userId = ((User)_contextAccessor.HttpContext.Items["User"]).Id;

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
