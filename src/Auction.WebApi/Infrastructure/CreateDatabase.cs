using Npgsql;

namespace Auction.WebApi.Infrastructure;

public static class CreateDatabase
{
    public static void CreateDataBaseIfNotExists(string connectionString)
    {
        var connStrParams = connectionString.Split(';');
        string pgConnStr = string.Empty;
        foreach (var param in connStrParams)
        {
            var check = param.StartsWith("Database=");
            pgConnStr += check ? string.Empty : $"{param};";
        }

        string sql = "select 1 from postgres.pg_catalog.pg_database where datname = 'auction'";
        string sql2 = "CREATE DATABASE \"auction\"";
        using NpgsqlConnection cnn = new NpgsqlConnection(pgConnStr);
        cnn.Open();

        var selectCommand = cnn.CreateCommand();
        selectCommand.CommandText = sql;
        var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            return;
        }
        cnn.Close();
        cnn.Open();

        var command = cnn.CreateCommand();
        command.CommandText = sql2;
        command.ExecuteNonQuery();
        cnn.Close();
    }
}