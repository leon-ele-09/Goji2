namespace GojiApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PostgresConnectionFactory factory)
        {
            using var connection = factory.CreateOpenConnection();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText =
                """
                SELECT pg_advisory_xact_lock(727274);

                CREATE TABLE IF NOT EXISTS Users (
                    Id UUID PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Active BOOLEAN NOT NULL,
                    CreatedAt TIMESTAMPTZ NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Projects (
                    Id UUID PRIMARY KEY,
                    BusinessKey TEXT,
                    Name TEXT NOT NULL,
                    CreatedAt TIMESTAMPTZ NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ProjectUsers (
                    ProjectId UUID NOT NULL REFERENCES Projects(Id) ON DELETE CASCADE,
                    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                    PRIMARY KEY (ProjectId, UserId)
                );

                CREATE TABLE IF NOT EXISTS TaskItems (
                    Id UUID PRIMARY KEY,
                    Name TEXT NOT NULL,
                    ProjectId UUID NOT NULL REFERENCES Projects(Id) ON DELETE CASCADE,
                    AssigneeId UUID NOT NULL REFERENCES Users(Id),
                    Description TEXT,
                    Status TEXT,
                    Priority TEXT,
                    CreatedAt TIMESTAMPTZ NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Alarms (
                    Id UUID PRIMARY KEY,
                    TaskItemId UUID NOT NULL REFERENCES TaskItems(Id) ON DELETE CASCADE,
                    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                    TriggerAtUtc TIMESTAMPTZ NOT NULL,
                    Status TEXT NOT NULL,
                    FiredAtUtc TIMESTAMPTZ,
                    LockedUntilUtc TIMESTAMPTZ,
                    CreatedAt TIMESTAMPTZ NOT NULL,
                    CONSTRAINT CK_Alarms_Status
                        CHECK (Status IN ('Pending', 'Processing', 'Fired', 'Cancelled'))
                );

                CREATE INDEX IF NOT EXISTS IX_Alarms_Due
                    ON Alarms (TriggerAtUtc)
                    WHERE Status IN ('Pending', 'Processing');
                """;

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}