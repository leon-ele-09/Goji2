using GojiApi.Data;
using GojiApi.Model.Project;
using Microsoft.Data.Sqlite;

namespace GojiApi.Repository
{
    public class ProjectRepository : IProject
    {
        private readonly SqliteConnectionFactory _factory;

        public ProjectRepository(SqliteConnectionFactory factory)
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
                    VALUES ($id, $businessKey, $name, $createdAt);
                    """;
                command.Parameters.AddWithValue("$id", project.Id.ToString());
                command.Parameters.AddWithValue("$businessKey", (object?)project.BusinessKey ?? DBNull.Value);
                command.Parameters.AddWithValue("$name", project.Name ?? string.Empty);
                command.Parameters.AddWithValue("$createdAt", (project.CreatedAt ?? DateTime.Now).ToString("O"));
                command.ExecuteNonQuery();
            }

            InsertMembers(connection, transaction, project.Id, project.UserIds);

            transaction.Commit();
        }

        public Project? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id.ToString());

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
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE Name = $name LIMIT 1;";
            command.Parameters.AddWithValue("$name", name);

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
            command.CommandText = "SELECT Id, BusinessKey, Name, CreatedAt FROM Projects WHERE BusinessKey = $businessKey LIMIT 1;";
            command.Parameters.AddWithValue("$businessKey", businessKey);

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
                    SET BusinessKey = $businessKey, Name = $name
                    WHERE Id = $id;
                    """;
                command.Parameters.AddWithValue("$id", project.Id.ToString());
                command.Parameters.AddWithValue("$businessKey", (object?)project.BusinessKey ?? DBNull.Value);
                command.Parameters.AddWithValue("$name", project.Name ?? string.Empty);
                command.ExecuteNonQuery();
            }

            using (var clearCommand = connection.CreateCommand())
            {
                clearCommand.Transaction = transaction;
                clearCommand.CommandText = "DELETE FROM ProjectUsers WHERE ProjectId = $id;";
                clearCommand.Parameters.AddWithValue("$id", project.Id.ToString());
                clearCommand.ExecuteNonQuery();
            }

            InsertMembers(connection, transaction, project.Id, project.UserIds);

            transaction.Commit();
        }

        public void Delete(Project project)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Projects WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", project.Id.ToString());
            command.ExecuteNonQuery();
        }

        private static void InsertMembers(SqliteConnection connection, SqliteTransaction transaction, Guid projectId, IEnumerable<Guid> userIds)
        {
            foreach (var userId in userIds.Distinct())
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    """
                    INSERT OR IGNORE INTO ProjectUsers (ProjectId, UserId)
                    VALUES ($projectId, $userId);
                    """;
                command.Parameters.AddWithValue("$projectId", projectId.ToString());
                command.Parameters.AddWithValue("$userId", userId.ToString());
                command.ExecuteNonQuery();
            }
        }

        private static List<Guid> GetMemberIds(SqliteConnection connection, Guid projectId)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT UserId FROM ProjectUsers WHERE ProjectId = $projectId;";
            command.Parameters.AddWithValue("$projectId", projectId.ToString());

            var ids = new List<Guid>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(Guid.Parse(reader.GetString(0)));
            }
            return ids;
        }

        private static Project Map(SqliteDataReader reader) =>
            Project.Hydrate(
                id: Guid.Parse(reader.GetString(0)),
                name: reader.GetString(2),
                businessKey: reader.IsDBNull(1) ? null : reader.GetString(1),
                userIds: new List<Guid>(),
                createdAt: DateTime.Parse(reader.GetString(3)));
    }
}
