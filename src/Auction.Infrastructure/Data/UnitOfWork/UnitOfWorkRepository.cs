using Auction.Application.Common.Abstractions.Repository;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Infrastructure.Data.Repositories;
using Npgsql;

namespace Auction.Infrastructure.Data.UnitOfWork;

public class UnitOfWorkRepository : IUnitOfWorkRepository
{
    public ILotRepository LotRepository { get; set; }
    public IUserRepository UserRepository { get; set; }
    public IAccountRepository AccountRepository { get; set; }
    public IUserBetRepository UserBetRepository { get; set; }

    public UnitOfWorkRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        LotRepository = new LotRepository(connection, transaction);
        UserRepository = new UserRepository(connection, transaction);
        AccountRepository = new AccountRepository(connection, transaction);
        UserBetRepository = new UserBetsRepository(connection, transaction);
    }
}
