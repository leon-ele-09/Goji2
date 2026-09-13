using GojiApi.Data;
using GojiApi.Model.TaskItem;
using Microsoft.Data.Sqlite;

namespace GojiApi.Repository
{
    public class TaskItemRepository : ITaskItem
    {
        private const string SelectColumns =
            "Id, Name, ProjectId, AssigneeId, Description, Status, Priority, CreatedAt";

        private readonly SqliteConnectionFactory _factory;

        public TaskItemRepository(SqliteConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Add(TaskItem taskItem)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                $"""
                INSERT INTO TaskItems (Id, Name, ProjectId, AssigneeId, Description, Status, Priority, CreatedAt)
                VALUES ($id, $name, $projectId, $assigneeId, $description, $status, $priority, $createdAt);
                """;

            command.Parameters.AddWithValue("$id", taskItem.Id.ToString());
            command.Parameters.AddWithValue("$name", taskItem.Name ?? string.Empty);
            command.Parameters.AddWithValue("$projectId", taskItem.ProjectId.ToString());
            command.Parameters.AddWithValue("$assigneeId", taskItem.AssigneeId.ToString());
            command.Parameters.AddWithValue("$description", (object?)taskItem.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("$status", (object?)taskItem.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("$priority", (object?)taskItem.Priority ?? DBNull.Value);
            command.Parameters.AddWithValue("$createdAt", (taskItem.CreatedAt ?? DateTime.UtcNow).ToString("O"));

            command.ExecuteNonQuery();
        }

        public TaskItem? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT {SelectColumns} FROM TaskItems WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id.ToString());

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public TaskItem? GetByName(string name)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT {SelectColumns} FROM TaskItems WHERE Name = $name LIMIT 1;";
            command.Parameters.AddWithValue("$name", name);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public TaskItem? GetByDescription(string description)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT {SelectColumns} FROM TaskItems WHERE Description = $description LIMIT 1;";
            command.Parameters.AddWithValue("$description", description);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<TaskItem> GetByStatus(string status) =>
            Query($"SELECT {SelectColumns} FROM TaskItems WHERE Status = $value;", status);

        public IEnumerable<TaskItem> GetByPriority(string priority) =>
            Query($"SELECT {SelectColumns} FROM TaskItems WHERE Priority = $value;", priority);

        public IEnumerable<TaskItem> GetByProjectId(Guid projectId) =>
            Query($"SELECT {SelectColumns} FROM TaskItems WHERE ProjectId = $value;", projectId.ToString());

        public IEnumerable<TaskItem> GetByAssigneeId(Guid assigneeId) =>
            Query($"SELECT {SelectColumns} FROM TaskItems WHERE AssigneeId = $value;", assigneeId.ToString());

        public void Update(TaskItem taskItem)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                UPDATE TaskItems
                SET Name = $name, ProjectId = $projectId, AssigneeId = $assigneeId,
                    Description = $description, Status = $status, Priority = $priority
                WHERE Id = $id;
                """;

            command.Parameters.AddWithValue("$id", taskItem.Id.ToString());
            command.Parameters.AddWithValue("$name", taskItem.Name ?? string.Empty);
            command.Parameters.AddWithValue("$projectId", taskItem.ProjectId.ToString());
            command.Parameters.AddWithValue("$assigneeId", taskItem.AssigneeId.ToString());
            command.Parameters.AddWithValue("$description", (object?)taskItem.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("$status", (object?)taskItem.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("$priority", (object?)taskItem.Priority ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Delete(TaskItem taskItem)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TaskItems WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", taskItem.Id.ToString());
            command.ExecuteNonQuery();
        }

        private List<TaskItem> Query(string sql, string value)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("$value", value);

            var items = new List<TaskItem>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(Map(reader));
            }
            return items;
        }

        private static TaskItem Map(SqliteDataReader reader) =>
            TaskItem.Hydrate(
                id: Guid.Parse(reader.GetString(0)),
                name: reader.GetString(1),
                projectId: Guid.Parse(reader.GetString(2)),
                assigneeId: Guid.Parse(reader.GetString(3)),
                description: reader.IsDBNull(4) ? null : reader.GetString(4),
                status: reader.IsDBNull(5) ? null : reader.GetString(5),
                priority: reader.IsDBNull(6) ? null : reader.GetString(6),
                createdAt: DateTime.Parse(reader.GetString(7)));


    }
}
