using Microsoft.Data.Sqlite;

namespace GojiApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SqliteConnectionFactory factory)
        {
            using var connection = factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS Users (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Active INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Projects (
                    Id TEXT PRIMARY KEY,
                    BusinessKey TEXT,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ProjectUsers (
                    ProjectId TEXT NOT NULL,
                    UserId TEXT NOT NULL,
                    PRIMARY KEY (ProjectId, UserId),
                    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS TaskItems (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    ProjectId TEXT NOT NULL,
                    AssigneeId TEXT NOT NULL,
                    Description TEXT,
                    Status TEXT,
                    Priority TEXT,
                    CreatedAt TEXT NOT NULL,
                    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
                    FOREIGN KEY (AssigneeId) REFERENCES Users(Id)
                );
                """;

            command.ExecuteNonQuery();
        }
    }
}
