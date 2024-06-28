using Auction.Application.Common.Abstractions.UnitOfWork;

namespace Auction.Infrastructure.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;

    public UnitOfWork(string connectionString) => _connectionString = connectionString;

    public IUnitOfWorkAdapter Create()
    {
        return new UnitOfWorkAdapter(_connectionString);
    }
}
