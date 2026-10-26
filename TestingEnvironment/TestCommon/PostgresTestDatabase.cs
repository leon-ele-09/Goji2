using GojiApi.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace TestingEnvironment.TestCommon
{
    /// <summary>
    /// Crea un schema aislado en Postgres para un test y lo borra al terminar.
    /// </summary>
    public sealed class PostgresTestDatabase : IDisposable
    {
        private const string EnvVar = "TEST_POSTGRES";

        private readonly string _baseConnectionString;
        private readonly string _schema = $"test_{Guid.NewGuid():N}";

        public PostgresConnectionFactory Factory { get; }

        public PostgresTestDatabase(string createTablesSql)
        {
            _baseConnectionString = Environment.GetEnvironmentVariable(EnvVar)
                ?? throw new InvalidOperationException(
                    $"Define la variable de entorno {EnvVar} con la cadena de conexion de una base de pruebas (no la de desarrollo).");

            Execute(_baseConnectionString, $"CREATE SCHEMA \"{_schema}\";");

            var testConnectionString = new NpgsqlConnectionStringBuilder(_baseConnectionString)
            {
                SearchPath = _schema
            }.ConnectionString;

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] = testConnectionString
                }).Build();

            Factory = new PostgresConnectionFactory(configuration);

            using var connection = Factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = createTablesSql;
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Factory.Dispose();
            Execute(_baseConnectionString, $"DROP SCHEMA IF EXISTS \"{_schema}\" CASCADE;");
        }

        private static void Execute(string connectionString, string sql)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}