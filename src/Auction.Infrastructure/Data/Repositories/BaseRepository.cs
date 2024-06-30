using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Common;
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

    public virtual async Task<TEntity?> GetById(Guid id)
    {
        NpgsqlCommand selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<TEntity>();
        var idFieldName = mapper.GetFieldName(nameof(BaseEntity.Id));

        var query = $"{GetAllQuery()} WHERE {idFieldName} = @{idFieldName}";

        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{idFieldName}", id);

        var result = await ReadData(selectCommand);

        return result.SingleOrDefault();
    }

    public virtual async Task<IList<TEntity>> GetAll()
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<TEntity>();
        var tableName = mapper.GetTableName();
        var fields = mapper.GetMapping().Values;

        var tableFieldsStr = string.Join(", ", fields);

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

    public virtual async Task<IList<TEntity>> GetBy(string entityPropName, object value)
    {
        NpgsqlCommand selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<TEntity>();
        var queryFieldName = mapper.GetFieldName(entityPropName);

        var query = $"{GetAllQuery()} WHERE {queryFieldName} = @{queryFieldName}";
        selectCommand.CommandText = query;
        selectCommand.Parameters.AddWithValue($"@{queryFieldName}", value);

        var result = await ReadData(selectCommand);

        return result;
    }

    public virtual async Task<Guid> Create(TEntity entity)
    {
        var insertCommand = _connection.CreateCommand();
        insertCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<TEntity>();
        var tableName = mapper.GetTableName();
        var mapping = mapper.GetMapping();
        var fields = mapping.Values;

        var tableFieldsStr = string.Join(", ", fields);
        var valuesParamsStr = string.Join(", ", fields.Select(x => $"@{x}"));

        var query = $"INSERT INTO {tableName} ({tableFieldsStr}) VALUES ({valuesParamsStr})";

        insertCommand.CommandText = query;

        var entityType = entity.GetType();
        Guid entityId = Guid.NewGuid();

        foreach (var field in mapping)
        {
            var entityPropName = field.Key;
            var propValue = entityPropName == nameof(BaseEntity.Id) ? entityId : GetPropValue(entity, entityPropName);

            insertCommand.Parameters.AddWithValue($"@{field.Value}", propValue);
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
        var idFieldName = mapper.GetFieldName(nameof(BaseEntity.Id));

        var query = $"DELETE FROM {tableName} WHERE {idFieldName} = @{idFieldName}";

        deleteCommand.CommandText = query;
        deleteCommand.Parameters.AddWithValue($"@{idFieldName}", id);
        await deleteCommand.ExecuteNonQueryAsync();
    }

    public virtual async Task Update(TEntity entity)
    {
        var updateCommand = _connection.CreateCommand();
        updateCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<TEntity>();
        var tableName = mapper.GetTableName();
        var fields = mapper.GetMapping();
        var idFieldName = mapper.GetFieldName(nameof(BaseEntity.Id));

        var setFields = string.Join(", ", fields
            .Select(x => $"{x.Value}=@{x.Value}"));

        var query = $"UPDATE {tableName} SET {setFields} WHERE {idFieldName} = @{idFieldName}";

        updateCommand.CommandText = query;

        var entityType = entity.GetType();

        foreach (var field in fields)
        {
            var propName = field.Key;
            var fieldName = field.Value;
            var propValue = GetPropValue(entity, propName);
            
            updateCommand.Parameters.AddWithValue($"@{fieldName}", propValue);
        }

        await updateCommand.ExecuteNonQueryAsync();
    }

    protected abstract TEntity Read(NpgsqlDataReader reader);

    protected object GetPropValue(TEntity entity, string propName)
    {
        object? propValue = null;
        var prop = typeof(TEntity).GetProperty(propName);
        if (prop is not null)
        {
            propValue = prop.GetValue(entity, null);
            if (prop.PropertyType.IsEnum && propValue != null)
            {
                propValue = (int)propValue;
            }
        }

        return propValue ?? DBNull.Value;
    }

    protected string GetAllQuery()
    {
        var mapper = Mapper.GetMapper<TEntity>();
        var tableName = mapper.GetTableName();
        var tableFieldsStr = string.Join(", ", mapper.GetMapping().Values);
        var query = $"SELECT {tableFieldsStr} FROM {tableName}";
        return query;
    }

    protected async Task<List<TEntity>> ReadData(NpgsqlCommand command)
    {
        using var reader = await command.ExecuteReaderAsync();
        List<TEntity> result = new(Convert.ToInt32(reader.Rows));
        while (reader.Read())
        {
            result.Add(Read(reader));
        }
        return result;
    }
}
