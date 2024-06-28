using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Common;
using Auction.Infrastructure.Data.Constants;
using Auction.Infrastructure.Data.Utils;
using AutoMapper.Execution;
using Microsoft.Data.Sqlite;

namespace Auction.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly SqliteConnection _connection;
    protected readonly SqliteTransaction _transaction;

    public BaseRepository(SqliteConnection connection, SqliteTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    //public virtual IEnumerable<TEntity> Get()
    //{
    //    using (var connection = new SqliteConnection("" + new SqliteConnectionStringBuilder
    //    {
    //        DataSource = "auction.db"
    //    }))
    //    {
    //        connection.Open();

    //        using (var transaction = connection.BeginTransaction())
    //        {
    //            var insertCommand = connection.CreateCommand();
    //            insertCommand.Transaction = transaction;
    //            insertCommand.CommandText = "INSERT INTO message ( text ) VALUES ( $text )";
    //            insertCommand.Parameters.AddWithValue("$text", "Hello, World!");
    //            insertCommand.ExecuteNonQuery();

    //            var selectCommand = connection.CreateCommand();
    //            selectCommand.Transaction = transaction;
    //            selectCommand.CommandText = "SELECT text FROM message";
    //            using (var reader = selectCommand.ExecuteReader())
    //            {
    //                while (reader.Read())
    //                {
    //                    var message = reader.GetString(0);
    //                    Console.WriteLine(message);
    //                }
    //            }

    //            transaction.Commit();
    //        }
    //    }
    //}

    public virtual async Task<IList<TEntity>> GetAll()
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;
        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        mapping.Remove(EntityConstants.TableName);

        var keysArr = mapping.Keys.ToArray();
        var fieldsArr = keysArr.Select(x => mapping[x]).ToArray();

        var tableFieldsStr = string.Join(", ", fieldsArr);

        var query = $"SELECT {tableFieldsStr} FROM {tableName}";

        selectCommand.CommandText = query;

        using var reader = await selectCommand.ExecuteReaderAsync();
        List<TEntity> result = [];
        while (reader.Read())
        {
            TEntity entity = Read(reader);
            result.Add(entity);
        }

        return result;
    }

    protected abstract TEntity Read(SqliteDataReader reader);

    public virtual async Task Create(TEntity entity)
    {
        var insertCommand = _connection.CreateCommand();
        insertCommand.Transaction = _transaction;

        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        var fields = mapping.Where(x => x.Key != EntityConstants.TableName);

        var tableFieldsStr = string.Join(", ", fields.Select(x => x.Value));
        var valuesParamsStr = string.Join(", ", fields.Select(x => $"${x.Value}"));

        var query = $"INSERT INTO {tableName} ({tableFieldsStr}) VALUES ({valuesParamsStr})";

        insertCommand.CommandText = query;

        var entityType = entity.GetType();

        foreach (var field in fields)
        {
            var prop = entityType.GetProperty(field.Key);
            if (prop != null)
            {
                var propValue = prop.GetValue(entity, null);
                insertCommand.Parameters.AddWithValue($"${field.Value}", field.Key == nameof(BaseEntity.Id) ? Guid.NewGuid() : propValue);
            }
        }

        await insertCommand.ExecuteNonQueryAsync();
    }
}
