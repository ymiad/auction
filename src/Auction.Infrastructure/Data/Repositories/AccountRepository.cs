using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public AccountRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction)
    {
    }

    protected override Account Read(NpgsqlDataReader reader)
    {
        var mapper = Mapper.GetMapper<Account>();

        var result = new Account
        {
            Id = reader.GetGuid(mapper.GetFieldName(nameof(Account.Id))),
            Ammount = reader.GetDecimal(mapper.GetFieldName(nameof(Account.Ammount))),
        };

        return result;
    }
}
