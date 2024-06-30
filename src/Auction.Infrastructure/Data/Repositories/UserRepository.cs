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
        var mapper = Mapper.GetMapper<User>();

        var result = new User
        {
            Id = reader.GetGuid(mapper.GetFieldName(nameof(User.Id))),
            Username = reader.GetString(mapper.GetFieldName(nameof(User.Username))),
            Password = reader.GetString(mapper.GetFieldName(nameof(User.Password))),
            PasswordSalt = reader.GetString(mapper.GetFieldName(nameof(User.PasswordSalt))),
            AccountId = reader.GetGuid(mapper.GetFieldName(nameof(User.AccountId))),
            Role = (Role)reader.GetInt32(mapper.GetFieldName(nameof(User.Role))),
            Banned = reader.GetBoolean(mapper.GetFieldName(nameof(User.Banned))),
        };

        return result;
    }

    public async Task<User?> GetByCredentials(string username, string password)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.Transaction = _transaction;

        var mapper = Mapper.GetMapper<User>();

        string usernameField = mapper.GetFieldName(nameof(User.Username));
        string passField = mapper.GetFieldName(nameof(User.Password));

        var query = $"{GetAllQuery()} WHERE {usernameField} = @{usernameField} AND {passField} = @{passField}";


        selectCommand.CommandText = query;

        selectCommand.Parameters.AddWithValue($"@{usernameField}", username);
        selectCommand.Parameters.AddWithValue($"@{passField}", password);

        var result = await ReadData(selectCommand);

        return result.SingleOrDefault();
    }
}
