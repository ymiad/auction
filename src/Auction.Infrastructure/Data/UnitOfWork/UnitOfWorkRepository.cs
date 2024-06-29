using Auction.Application.Common.Abstractions.Repository;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Infrastructure.Data.Repositories;
using Microsoft.Data.Sqlite;

namespace Auction.Infrastructure.Data.UnitOfWork;

public class UnitOfWorkRepository : IUnitOfWorkRepository
{
    public ILotRepository LotRepository { get; set; }
    public IUserRepository UserRepository { get; set; }

    public UnitOfWorkRepository(SqliteConnection connection, SqliteTransaction transaction)
    {
        LotRepository = new LotRepository(connection, transaction);
        UserRepository = new UserRepository(connection, transaction);
    }
}
