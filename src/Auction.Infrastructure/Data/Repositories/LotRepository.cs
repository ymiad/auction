using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories
{
    public class LotRepository : BaseRepository<Lot>, ILotRepository
    {
        public LotRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

        protected override Lot Read(NpgsqlDataReader reader)
        {
            var mapping = Mapper.GetMap<Lot>();

            var result = new Lot
            {
                Id = reader.GetGuid(mapping[nameof(Lot.Id)]),
                Name = reader.GetString(mapping[nameof(Lot.Name)]),
                Description = reader.GetString(mapping[nameof(Lot.Description)]),
                StartPrice = reader.GetDecimal(mapping[nameof(Lot.StartPrice)]),
                TradingStartDate = reader.GetDateTime(mapping[nameof(Lot.TradingStartDate)]),
                TradingEndDate = reader.GetDateTime(mapping[nameof(Lot.TradingEndDate)]),
            };

            return result;
        }
    }
}
