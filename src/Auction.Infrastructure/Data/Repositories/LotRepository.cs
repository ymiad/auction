using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Mapping;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories
{
    public class LotRepository : BaseRepository<Lot>, ILotRepository
    {
        public LotRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

        public async Task<IList<Lot>> GetCurrentLots()
        {
            var selectCommand = _connection.CreateCommand();
            selectCommand.Transaction = _transaction;
            var lotMapping = new LotMapping();
            string tableName = lotMapping.GetTableName();
            Dictionary<string, string> fieldsMapping = lotMapping.GetFields();

            var tradingStarDateField = lotMapping.GetTableFieldName(nameof(Lot.TradingStartDate));
            var archivedField = lotMapping.GetTableFieldName(nameof(Lot.Archived));
            var tradingEndDateField = lotMapping.GetTableFieldName(nameof(Lot.TradingEndDate));
            var ownerField = lotMapping.GetTableFieldName(nameof(Lot.OwnerId));

            var query = $"{GetAllQuery()}" +
                $" WHERE {tradingStarDateField} <= @{tradingStarDateField}" +
                $" AND {tradingEndDateField} >= @{tradingEndDateField}" +
                $" AND NOT {archivedField}" +
                $" AND {ownerField} <> @{ownerField}";

            selectCommand.CommandText = query;

            selectCommand.Parameters.AddWithValue($"@{tradingStarDateField}", DateTime.Now);
            selectCommand.Parameters.AddWithValue($"@{tradingEndDateField}", DateTime.Now);
            selectCommand.Parameters.AddWithValue($"@{archivedField}", false);
            selectCommand.Parameters.AddWithValue($"@{ownerField}", Guid.Empty);

            using var reader = await selectCommand.ExecuteReaderAsync();

            List<Lot> result = new(Convert.ToInt32(reader.Rows));

            while (reader.Read())
            {
                result.Add(Read(reader));
            }

            return result;
        }

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
                Archived = reader.GetBoolean(mapping[nameof(Lot.Archived)]),
                OwnerId = reader.GetGuid(mapping[nameof(Lot.OwnerId)]),
                PublisherId = reader.GetGuid(mapping[nameof(Lot.PublisherId)]),
            };

            return result;
        }
    }
}
