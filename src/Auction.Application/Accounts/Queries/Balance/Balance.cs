using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Auction.Application.Accounts.Queries.Ammount;

[Authorize]
public record AmmountQuery : IRequest<decimal>;

public class AmmountQueryHandler : IRequestHandler<AmmountQuery, decimal>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AmmountQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<decimal> Handle(AmmountQuery request, CancellationToken cancellationToken)
    {
        var userId = ((User)_httpContextAccessor.HttpContext.Items["User"]).Id;

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
