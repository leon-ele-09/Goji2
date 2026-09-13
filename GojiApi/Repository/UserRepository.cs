using GojiApi.Data;
using GojiApi.Model.User;
using Microsoft.Data.Sqlite;

namespace GojiApi.Repository
{
    public class UserRepository : IUser
    {
        private readonly SqliteConnectionFactory _factory;

        public UserRepository(SqliteConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Add(User user)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                INSERT INTO Users (Id, Name, Email, Active, CreatedAt)
                VALUES ($id, $name, $email, $active, $createdAt);
                """;

            command.Parameters.AddWithValue("$id", user.Id.ToString());
            command.Parameters.AddWithValue("$name", user.Name ?? string.Empty);
            command.Parameters.AddWithValue("$email", user.Email ?? string.Empty);
            command.Parameters.AddWithValue("$active", user.Active ? 1 : 0);
            command.Parameters.AddWithValue("$createdAt", (user.CreatedAt ?? DateTime.UtcNow).ToString("O"));

            command.ExecuteNonQuery();
        }

        public User? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id.ToString());

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public User? GetByName(string name)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Name = $name LIMIT 1;";
            command.Parameters.AddWithValue("$name", name);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public User? GetByEmail(string email)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Email = $email LIMIT 1;";
            command.Parameters.AddWithValue("$email", email);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public void Update(User user)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                UPDATE Users
                SET Name = $name, Email = $email, Active = $active
                WHERE Id = $id;
                """;

            command.Parameters.AddWithValue("$id", user.Id.ToString());
            command.Parameters.AddWithValue("$name", user.Name ?? string.Empty);
            command.Parameters.AddWithValue("$email", user.Email ?? string.Empty);
            command.Parameters.AddWithValue("$active", user.Active ? 1 : 0);

            command.ExecuteNonQuery();
        }

        public void Delete(User user)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", user.Id.ToString());
            command.ExecuteNonQuery();
        }

        private static User Map(SqliteDataReader reader) =>
            User.Hydrate(
                id: Guid.Parse(reader.GetString(0)),
                name: reader.GetString(1),
                email: reader.GetString(2),
                active: reader.GetInt32(3) == 1,
                createdAt: DateTime.Parse(reader.GetString(4)));
    }
}
