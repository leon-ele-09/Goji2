using GojiApi.Data;
using GojiApi.Model.Alarm;
using Npgsql;

namespace GojiApi.Repository
{
    public class AlarmRepository : IAlarm
    {
        private const string SelectColumns =
            "Id, TaskItemId, UserId, TriggerAtUtc, Status, FiredAtUtc, LockedUntilUtc, CreatedAt";

        private readonly PostgresConnectionFactory _factory;

        public AlarmRepository(PostgresConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Add(Alarm alarm)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                INSERT INTO Alarms (Id, TaskItemId, UserId, TriggerAtUtc, Status, FiredAtUtc, LockedUntilUtc, CreatedAt)
                VALUES (@id, @taskItemId, @userId, @triggerAt, @status, @firedAt, @lockedUntil, @createdAt);
                """;

            command.Parameters.AddWithValue("id", alarm.Id);
            command.Parameters.AddWithValue("taskItemId", alarm.TaskItemId);
            command.Parameters.AddWithValue("userId", alarm.UserId);
            command.Parameters.AddWithValue("triggerAt", alarm.TriggerAtUtc);
            command.Parameters.AddWithValue("status", alarm.Status.ToString());
            command.Parameters.AddWithValue("firedAt", (object?)alarm.FiredAtUtc ?? DBNull.Value);
            command.Parameters.AddWithValue("lockedUntil", (object?)alarm.LockedUntilUtc ?? DBNull.Value);
            command.Parameters.AddWithValue("createdAt", alarm.CreatedAt ?? DateTime.UtcNow);

            command.ExecuteNonQuery();
        }

        public Alarm? GetById(Guid id)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT {SelectColumns} FROM Alarms WHERE Id = @id;";
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<Alarm> GetByTaskItemId(Guid taskItemId) =>
            Query($"SELECT {SelectColumns} FROM Alarms WHERE TaskItemId = @value ORDER BY TriggerAtUtc;", taskItemId);

        public IEnumerable<Alarm> GetByUserId(Guid userId) =>
            Query($"SELECT {SelectColumns} FROM Alarms WHERE UserId = @value ORDER BY TriggerAtUtc;", userId);

        public void Update(Alarm alarm)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                UPDATE Alarms
                SET TriggerAtUtc = @triggerAt, Status = @status,
                    FiredAtUtc = @firedAt, LockedUntilUtc = @lockedUntil
                WHERE Id = @id;
                """;

            command.Parameters.AddWithValue("id", alarm.Id);
            command.Parameters.AddWithValue("triggerAt", alarm.TriggerAtUtc);
            command.Parameters.AddWithValue("status", alarm.Status.ToString());
            command.Parameters.AddWithValue("firedAt", (object?)alarm.FiredAtUtc ?? DBNull.Value);
            command.Parameters.AddWithValue("lockedUntil", (object?)alarm.LockedUntilUtc ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Delete(Alarm alarm)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Alarms WHERE Id = @id;";
            command.Parameters.AddWithValue("id", alarm.Id);
            command.ExecuteNonQuery();
        }

        // Reclamo atómico: solo una instancia obtiene cada alarma
        public IEnumerable<Alarm> ClaimDue(int batchSize, TimeSpan lockDuration)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                $"""
                UPDATE Alarms
                SET Status = 'Processing', LockedUntilUtc = now() + @lockDuration
                WHERE Id IN (
                    SELECT Id FROM Alarms
                    WHERE TriggerAtUtc <= now()
                      AND (Status = 'Pending'
                           OR (Status = 'Processing' AND LockedUntilUtc <= now()))
                    ORDER BY TriggerAtUtc
                    LIMIT @batchSize
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING {SelectColumns};
                """;

            command.Parameters.AddWithValue("lockDuration", lockDuration);
            command.Parameters.AddWithValue("batchSize", batchSize);

            var claimed = new List<Alarm>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                claimed.Add(Map(reader));
            }
            return claimed;
        }

        private List<Alarm> Query(string sql, object value)
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("value", value);

            var items = new List<Alarm>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(Map(reader));
            }
            return items;
        }

        private static Alarm Map(NpgsqlDataReader reader) =>
            Alarm.Hydrate(
                id: reader.GetGuid(0),
                taskItemId: reader.GetGuid(1),
                userId: reader.GetGuid(2),
                triggerAtUtc: reader.GetDateTime(3),
                status: Enum.Parse<AlarmStatus>(reader.GetString(4)),
                firedAtUtc: reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                lockedUntilUtc: reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                createdAt: reader.GetDateTime(7));
    }
}