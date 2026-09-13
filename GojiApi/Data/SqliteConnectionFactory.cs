using Microsoft.Data.Sqlite;

namespace GojiApi.Data
{
    /// <summary>
    /// Genera la conexion de SQLite, se va a cambiar cuando empezemos a usar Postgres
    /// </summary>
    public class SqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sqlite")
                ?? "Data Source=goji.db";
        }

        public SqliteConnection CreateOpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
