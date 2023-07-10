using Dapper;
using Npgsql;

namespace Allocate.Common.Database.Services;

public class DatabaseMigrationService
{
    public static void EnsureDatabaseExists(string connectionString)
    {
        var cb = new NpgsqlConnectionStringBuilder
        {
            ConnectionString = connectionString
        };
        // set the DB name to be checked
        var ensureDatabaseName = cb.Database;
        Console.WriteLine($"MIGRATOR DATABASE: {ensureDatabaseName}");
        // reset to the system db for this operation
        cb.Database = "postgres";
        // create the DB if it doesn't already exist
        using (var connection = new NpgsqlConnection(cb.ConnectionString))
        {
            var records = connection.Query("SELECT oid FROM postgres.pg_catalog.pg_database WHERE datname = @DatabaseName", new { DatabaseName = ensureDatabaseName });
            if (!records.Any())
            {
                Console.WriteLine($"NOT FOUND, CREATING DATABASE: {ensureDatabaseName}");
                connection.Execute($"CREATE DATABASE \"{ensureDatabaseName}\"");
            }
            NpgsqlConnection.ClearAllPools();
        }
    }
}
