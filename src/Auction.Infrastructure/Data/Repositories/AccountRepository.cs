using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction)
        {
        }

        protected override Account Read(NpgsqlDataReader reader)
        {
            var mapping = Mapper.GetMap<Account>();

            var result = new Account
            {
                Id = reader.GetGuid(mapping[nameof(Account.Id)]),
                Ammount = reader.GetDecimal(mapping[nameof(Account.Ammount)]),
            };

            return result;
        }
    }
}
