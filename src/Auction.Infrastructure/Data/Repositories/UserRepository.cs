using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

    protected override User Read(NpgsqlDataReader reader)
    {
        var mapping = Mapper.GetMap<User>();

        var result = new User
        {
            Id = reader.GetGuid(mapping[nameof(User.Id)]),
            Username = reader.GetString(mapping[nameof(User.Username)]),
            Password = reader.GetString(mapping[nameof(User.Password)]),
            AccountId = reader.GetGuid(mapping[nameof(User.AccountId)]),
            Role = (Roles)reader.GetInt32(mapping[nameof(User.Role)]),
        };

        return result;
    }

    public async Task<User?> GetByCredentials(string username, string password)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;
        var mapping = Mapper.GetMap<User>();

        string usernameFieldName = Mapper.GetTableFieldName(mapping, nameof(User.Username));
        string passwordFieldName = Mapper.GetTableFieldName(mapping, nameof(User.Password));

        var query = $"{GetAllQuery()} WHERE {usernameFieldName} = @{usernameFieldName} AND {passwordFieldName} = @{passwordFieldName}";


        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{usernameFieldName}", username);
        selectCommand.Parameters.AddWithValue($"@{passwordFieldName}", password);

        using var reader = await selectCommand.ExecuteReaderAsync();

        User? result = null;

        while (reader.Read())
        {
            result = Read(reader);
        }

        return result;
    }

    public async Task<User> GetByUsername(string username)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;
        var mapping = Mapper.GetMap<User>();

        string usernameFieldName = Mapper.GetTableFieldName(mapping, nameof(User.Username));

        var query = $"{GetAllQuery()} WHERE {usernameFieldName} = @{usernameFieldName}";

        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{usernameFieldName}", username);

        using var reader = await selectCommand.ExecuteReaderAsync();

        var result = Read(reader);

        return result;
    }

    //public async Task<User> GetByEmail(string email)
    //{
    //    var selectCommand = _connection.CreateCommand();
    //    selectCommand.Transaction = _transaction;
    //    var mapping = Mapper.GetMap<User>();

    //    string emailFieldName = Mapper.GetTableFieldName(mapping, nameof(User.Email));

    //    var query = $"{GetAllQuery()} WHERE {emailFieldName} = ${emailFieldName}";

    //    selectCommand.CommandText = query;

    //    selectCommand.Parameters.AddWithValue($"{emailFieldName}", email);

    //    using var reader = await selectCommand.ExecuteReaderAsync();

    //    User result = null;

    //    while (reader.Read())
    //    {
    //        result = Read(reader);
    //    }

    //    return result;
    //}
}
