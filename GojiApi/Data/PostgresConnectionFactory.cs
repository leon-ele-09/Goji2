using Npgsql;

namespace GojiApi.Data
{
    public class PostgresConnectionFactory : IDisposable
    {
        private readonly NpgsqlDataSource _dataSource;

        public PostgresConnectionFactory(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");

            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public NpgsqlConnection CreateOpenConnection() => _dataSource.OpenConnection();

        public void Dispose() => _dataSource.Dispose();
    }
}