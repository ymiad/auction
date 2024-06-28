using Auction.Application.Common.Abstractions.UnitOfWork;
using Microsoft.Data.Sqlite;

namespace Auction.Infrastructure.Data.UnitOfWork;

public class UnitOfWorkAdapter : IUnitOfWorkAdapter
{
    private readonly SqliteConnection _connection;

    private readonly SqliteTransaction _transaction;

    public IUnitOfWorkRepository Repositories { get; set; }

    public UnitOfWorkAdapter(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        _transaction = _connection.BeginTransaction();

        Repositories = new UnitOfWorkRepository(_connection, _transaction);
    }

    public void Dispose()
    {
        _transaction?.Dispose();

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }

        Repositories = null;
    }

    public void SaveChanges()
    {
        _transaction.Commit();
    }
}
