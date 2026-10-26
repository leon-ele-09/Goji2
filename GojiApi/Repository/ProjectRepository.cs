using GojiApi.Data;
using GojiApi.Model.Project;
using Npgsql;

namespace GojiApi.Repository
{
    public class ProjectRepository : IProject
    {
        private readonly PostgresConnectionFactory _factory;

        public ProjectRepository(PostgresConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Add(Project project)
        {
            using var connection = _factory.CreateOpenConnection();
            using var transaction = connection.BeginTransaction();

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
                    """
                    INSERT INTO Projects (Id, BusinessKey, Name, CreatedAt)
                    VALUES (@id, @businessKey, @name, @createdAt);
                    """;
                command.Parameters.AddWithValue("id", project.Id);
                command.Parameters.AddWithValue("businessKey", (object?)project.BusinessKey ?? DBNull.Value);
                command.Parameters.AddWithValue("name", project.Name ?? string.Empty);
                command.Parameters.AddWithValue("createdAt", project.CreatedAt ?? DateTime.UtcNow);
                command.ExecuteNonQuery();
            }

            InsertMembers(connection, transaction, project.Id, project.UserIds);

            transaction.Commit();
        }

        public Project? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE Id = @id;";
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;
            var project = Map(reader);
            reader.Close();

            project.UserIds.Clear();
            project.UserIds.AddRange(GetMemberIds(connection, id));
            return project;
        }

        public Project? GetByName(string name)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE Name = @name LIMIT 1;";
            command.Parameters.AddWithValue("name", name);

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;
            var project = Map(reader);
            reader.Close();

            project.UserIds.Clear();
            project.UserIds.AddRange(GetMemberIds(connection, project.Id));
            return project;
        }

        public Project? GetByBusinessKey(string businessKey)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE BusinessKey = @businessKey LIMIT 1;";
            command.Parameters.AddWithValue("businessKey", businessKey);

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;
            var project = Map(reader);
            reader.Close();

            project.UserIds.Clear();
            project.UserIds.AddRange(GetMemberIds(connection, project.Id));
            return project;
        }

        public void Update(Project project)
        {
            using var connection = _factory.CreateOpenConnection();
            using var transaction = connection.BeginTransaction();

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
                    """
                    UPDATE Projects
                    SET BusinessKey = @businessKey, Name = @name
                    WHERE Id = @id;
                    """;
                command.Parameters.AddWithValue("id", project.Id);
                command.Parameters.AddWithValue("businessKey", (object?)project.BusinessKey ?? DBNull.Value);
                command.Parameters.AddWithValue("name", project.Name ?? string.Empty);
                command.ExecuteNonQuery();
            }

            using (var clearCommand = connection.CreateCommand())
            {
                clearCommand.Transaction = transaction;
                clearCommand.CommandText = "DELETE FROM ProjectUsers WHERE ProjectId = @id;";
                clearCommand.Parameters.AddWithValue("id", project.Id);
                clearCommand.ExecuteNonQuery();
            }

            InsertMembers(connection, transaction, project.Id, project.UserIds);

            transaction.Commit();
        }

        public void Delete(Project project)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Projects WHERE Id = @id;";
            command.Parameters.AddWithValue("id", project.Id);
            command.ExecuteNonQuery();
        }

        private static void InsertMembers(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid projectId, IEnumerable<Guid> userIds)
        {
            foreach (var userId in userIds.Distinct())
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    """
                    INSERT INTO ProjectUsers (ProjectId, UserId)
                    VALUES (@projectId, @userId)
                    ON CONFLICT DO NOTHING;
                    """;
                command.Parameters.AddWithValue("projectId", projectId);
                command.Parameters.AddWithValue("userId", userId);
                command.ExecuteNonQuery();
            }
        }

        private static List<Guid> GetMemberIds(NpgsqlConnection connection, Guid projectId)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT UserId FROM ProjectUsers WHERE ProjectId = @projectId;";
            command.Parameters.AddWithValue("projectId", projectId);

            var ids = new List<Guid>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetGuid(0));
            }
            return ids;
        }

        private static Project Map(NpgsqlDataReader reader) =>
            Project.Hydrate(
                id: reader.GetGuid(0),
                name: reader.GetString(2),
                businessKey: reader.IsDBNull(1) ? null : reader.GetString(1),
                userIds: new List<Guid>(),
                createdAt: reader.GetDateTime(3));
    }
}