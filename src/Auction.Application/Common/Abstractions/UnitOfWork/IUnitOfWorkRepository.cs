using Auction.Application.Common.Abstractions.Repository;

namespace Auction.Application.Common.Abstractions.UnitOfWork;

public interface IUnitOfWorkRepository
{
    ILotRepository LotRepository { get; }
    IUserRepository UserRepository { get; }
    IAccountRepository AccountRepository { get; }
    IUserBetRepository UserBetRepository { get; }
}
