using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Common;
using Auction.Infrastructure.Data.Constants;
using Auction.Infrastructure.Data.Utils;
using Npgsql;

namespace Auction.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly NpgsqlConnection _connection;
    protected readonly NpgsqlTransaction _transaction;

    public BaseRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public virtual async Task<TEntity> GetById(Guid id)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;
        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        mapping.Remove(EntityConstants.TableName);

        var keysArr = mapping.Keys.ToArray();
        var fieldsArr = keysArr.Select(x => mapping[x]).ToArray();

        var tableFieldsStr = string.Join(", ", fieldsArr);

        string idFieldName = Mapper.GetTableFieldName(mapping, nameof(BaseEntity.Id));

        var query = $"{GetAllQuery()} WHERE {idFieldName} = @{idFieldName}";

        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{idFieldName}", id);

        using var reader = await selectCommand.ExecuteReaderAsync();

        TEntity result = null;

        while (reader.Read())
        {
            result = Read(reader);
        }


        return result;
    }

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

    protected abstract TEntity Read(NpgsqlDataReader reader);

    public virtual async Task<IList<TEntity>> GetBy(string fieldName, object value)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;
        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        mapping.Remove(EntityConstants.TableName);

        var keysArr = mapping.Keys.ToArray();
        var fieldsArr = keysArr.Select(x => mapping[x]).ToArray();

        var tableFieldsStr = string.Join(", ", fieldsArr);

        string queryFieldName = Mapper.GetTableFieldName(mapping, fieldName);

        var query = $"{GetAllQuery()} WHERE {queryFieldName} = @{queryFieldName}";

        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{queryFieldName}", value);

        using var reader = await selectCommand.ExecuteReaderAsync();

        List<TEntity> result = [];

        while (reader.Read())
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public virtual async Task<Guid> Create(TEntity entity)
    {
        var insertCommand = _connection.CreateCommand();
        insertCommand.Transaction = _transaction;

        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        var fields = mapping.Where(x => x.Key != EntityConstants.TableName);

        var tableFieldsStr = string.Join(", ", fields.Select(x => x.Value));
        var valuesParamsStr = string.Join(", ", fields.Select(x => $"@{x.Value}"));

        var query = $"INSERT INTO {tableName} ({tableFieldsStr}) VALUES ({valuesParamsStr})";

        insertCommand.CommandText = query;

        var entityType = entity.GetType();
        Guid entityId = Guid.NewGuid();

        foreach (var field in fields)
        {
            var prop = entityType.GetProperty(field.Key);
            if (prop != null)
            {
                var propValue = prop.GetValue(entity, null);
                if (prop.PropertyType.IsEnum && propValue != null)
                {
                    propValue = (int)propValue;
                }

                insertCommand.Parameters.AddWithValue($"@{field.Value}", field.Key == nameof(BaseEntity.Id) ? entityId : propValue);
            }
        }

        await insertCommand.ExecuteNonQueryAsync();

        return entityId;
    }

    public virtual async Task Delete(Guid id)
    {
        var deleteCommand = _connection.CreateCommand();
        deleteCommand.Transaction = _transaction;
        var mapper = Mapper.GetMapper<TEntity>();
        var tableName = mapper.GetTableName();
        var idFieldName = mapper.GetTableFieldName(nameof(BaseEntity.Id));

        var query = $"DELETE FROM {tableName} WHERE {idFieldName} = @{idFieldName}";

        deleteCommand.CommandText = query;

        deleteCommand.Parameters.AddWithValue($"@{idFieldName}", id);

        await deleteCommand.ExecuteNonQueryAsync();
    }

    public virtual async Task Update(TEntity entity)
    {
        var updateCommand = _connection.CreateCommand();
        updateCommand.Transaction = _transaction;

        var mapping = Mapper.GetMap<TEntity>();
        mapping.TryGetValue(EntityConstants.TableName , out var tableName);

        var fields = mapping.Where(x => x.Key != EntityConstants.TableName);

        string idFieldName = Mapper.GetTableFieldName(mapping, nameof(BaseEntity.Id));

        var setFields = string.Join(", ", fields
            .Where(x => x.Key != nameof(BaseEntity.Id))
            .Select(x => $"{x.Value}=@{x.Value}"));

        var query = $"UPDATE {tableName} SET {setFields} WHERE {idFieldName} = @{idFieldName}";

        updateCommand.CommandText = query;

        var entityType = entity.GetType();

        foreach (var field in fields)
        {
            var prop = entityType.GetProperty(field.Key);
            if (prop != null)
            {
                var propValue = prop.GetValue(entity, null);
                if (prop.PropertyType.IsEnum && propValue != null)
                {
                    propValue = (int)propValue;
                }
                updateCommand.Parameters.AddWithValue($"@{field.Value}", propValue);
            }
        }

        await updateCommand.ExecuteNonQueryAsync();
    }

    protected string GetAllQuery()
    {
        var mapping = Mapper.GetMap<TEntity>();

        mapping.TryGetValue(EntityConstants.TableName, out var tableName);

        mapping.Remove(EntityConstants.TableName);

        var keysArr = mapping.Keys.ToArray();
        var fieldsArr = keysArr.Select(x => mapping[x]).ToArray();

        var tableFieldsStr = string.Join(", ", fieldsArr);

        var query = $"SELECT {tableFieldsStr} FROM {tableName}";

        return query;
    }
}
