using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories;

public class LotRepository : BaseRepository<Lot>, ILotRepository
{
    public LotRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

    public async Task<IList<Lot>> GetCurrentLots()
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<Lot>();

        var tradingStarDateField = mapper.GetFieldName(nameof(Lot.TradingStartDate));
        var archivedField = mapper.GetFieldName(nameof(Lot.Archived));
        var tradingEndDateField = mapper.GetFieldName(nameof(Lot.TradingEndDate));
        var ownerField = mapper.GetFieldName(nameof(Lot.OwnerId));

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
        var mapper = Mapper.GetMapper<Lot>();

        var result = new Lot
        {
            Id = reader.GetGuid(mapper.GetFieldName(nameof(Lot.Id))),
            Name = reader.GetString(mapper.GetFieldName(nameof(Lot.Name))),
            Description = reader.GetString(mapper.GetFieldName(nameof(Lot.Description))),
            StartPrice = reader.GetDecimal(mapper.GetFieldName(nameof(Lot.StartPrice))),
            TradingStartDate = reader.GetDateTime(mapper.GetFieldName(nameof(Lot.TradingStartDate))),
            TradingEndDate = reader.GetDateTime(mapper.GetFieldName(nameof(Lot.TradingEndDate))),
            Archived = reader.GetBoolean(mapper.GetFieldName(nameof(Lot.Archived))),
            OwnerId = reader.GetGuid(mapper.GetFieldName(nameof(Lot.OwnerId))),
            PublisherId = reader.GetGuid(mapper.GetFieldName(nameof(Lot.PublisherId))),
        };

        return result;
    }
}
