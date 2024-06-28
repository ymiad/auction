using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories
{
    public class LotRepository : BaseRepository<Lot>, ILotRepository
    {
        public LotRepository(SqliteConnection connection, SqliteTransaction transaction) : base(connection, transaction) { }

        protected override Lot Read(SqliteDataReader reader)
        {
            var mapping = Mapper.GetMap<Lot>();

            var result = new Lot();

            result.Id = reader.GetGuid(mapping[nameof(result.Id)]);
            result.Name = reader.GetString(mapping[nameof(result.Name)]);
            result.Description = reader.GetString(mapping[nameof(result.Description)]);
            result.TradingStart = reader.GetDateTime(mapping[nameof(result.TradingStart)]);
            result.StartPrice = reader.GetDecimal(mapping[nameof(result.StartPrice)]);
            result.TradingDuration = reader.GetInt32(mapping[nameof(result.TradingDuration)]);

            return result;
        }
    }
}
