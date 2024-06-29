using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories
{
    public class UserBetsRepository : BaseRepository<UserBet>, IUserBetRepository
    {
        public UserBetsRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

        protected override UserBet Read(NpgsqlDataReader reader)
        {
            var mapping = Mapper.GetMap<UserBet>();

            var result = new UserBet
            {
                Id = reader.GetGuid(mapping[nameof(UserBet.Id)]),
                LotId = reader.GetGuid(mapping[nameof(UserBet.LotId)]),
                UserId = reader.GetGuid(mapping[nameof(UserBet.UserId)]),
                Ammount = reader.GetDecimal(mapping[nameof(UserBet.Ammount)]),
            };

            return result;
        }
    }
}
