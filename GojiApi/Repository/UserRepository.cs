using GojiApi.Data;
using GojiApi.Model.User;
using Npgsql;

namespace GojiApi.Repository
{
    public class UserRepository : IUser
    {
        private readonly PostgresConnectionFactory _factory;

        public UserRepository(PostgresConnectionFactory factory)
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
                VALUES (@id, @name, @email, @active, @createdAt);
                """;

            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("name", user.Name ?? string.Empty);
            command.Parameters.AddWithValue("email", user.Email ?? string.Empty);
            command.Parameters.AddWithValue("active", user.Active);
            command.Parameters.AddWithValue("createdAt", user.CreatedAt ?? DateTime.UtcNow);

            command.ExecuteNonQuery();
        }

        public User? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Id = @id;";
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public User? GetByName(string name)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Name = @name LIMIT 1;";
            command.Parameters.AddWithValue("name", name);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public User? GetByEmail(string email)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email, Active, CreatedAt FROM Users WHERE Email = @email LIMIT 1;";
            command.Parameters.AddWithValue("email", email);

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
                SET Name = @name, Email = @email, Active = @active
                WHERE Id = @id;
                """;

            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("name", user.Name ?? string.Empty);
            command.Parameters.AddWithValue("email", user.Email ?? string.Empty);
            command.Parameters.AddWithValue("active", user.Active);

            command.ExecuteNonQuery();
        }

        public void Delete(User user)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @id;";
            command.Parameters.AddWithValue("id", user.Id);
            command.ExecuteNonQuery();
        }

        private static User Map(NpgsqlDataReader reader) =>
            User.Hydrate(
                id: reader.GetGuid(0),
                name: reader.GetString(1),
                email: reader.GetString(2),
                active: reader.GetBoolean(3),
                createdAt: reader.GetDateTime(4));
    }
}