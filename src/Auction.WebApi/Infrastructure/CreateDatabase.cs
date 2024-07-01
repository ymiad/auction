using Npgsql;

namespace Auction.WebApi.Infrastructure;

public static class CreateDatabase
{
    public static void CreateDataBaseIfNotExists(string connectionString)
    {
        var connStrParams = connectionString.Split(';');
        string pgConnStr = string.Empty;
        string dbName = string.Empty;
        foreach (var param in connStrParams)
        {
            var check = param.StartsWith("Database=");
            if (check)
            {
                dbName = param.Split('=')[1];
            }
            pgConnStr += check ? string.Empty : $"{param};";
        }

        string dbExistsSql = $"select 1 from postgres.pg_catalog.pg_database where datname = '{dbName}'";
        string createDbSql = $"CREATE DATABASE \"{dbName}\"";
        using NpgsqlConnection cnn = new NpgsqlConnection(pgConnStr);
        cnn.Open();

        var selectCommand = cnn.CreateCommand();
        selectCommand.CommandText = dbExistsSql;
        var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            return;
        }
        cnn.Close();
        cnn.Open();

        var command = cnn.CreateCommand();
        command.CommandText = createDbSql;
        command.ExecuteNonQuery();
        cnn.Close();
    }
}