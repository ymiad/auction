using Auction.Application.Common.Abstractions.UnitOfWork;
using Npgsql;

namespace Auction.Infrastructure.Data.UnitOfWork;

public class UnitOfWorkAdapter : IUnitOfWorkAdapter
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public IUnitOfWorkRepository Repositories { get; set; }

    public UnitOfWorkAdapter(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();

        _transaction = _connection.BeginTransaction();

        Repositories = new UnitOfWorkRepository(_connection, _transaction);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        Repositories = null!;
    }

    public void Dispose()
    {
        _transaction?.Dispose();

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }

        Repositories = null!;
    }

    public async Task SaveChangesAsync()
    {
        await _transaction.CommitAsync();
    }
}
