using GojiApi.Data;
using GojiApi.Model.TaskItem;
using GojiApi.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace TestingEnvironment.Repo
{
    [TestFixture]
    public class TaskItemRepositoryTests
    {
        private TaskItemRepository _repository = null!;
        private SqliteConnectionFactory _factory = null!;
        private string _databasePath = null!;

        [SetUp]
        public void Setup()
        {
            _databasePath = Path.Combine(
                Path.GetTempPath(),
                $"goji_test_{Guid.NewGuid()}.db"
            );

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Sqlite"] =
                        $"Data Source={_databasePath}"
                }).Build();

            _factory = new SqliteConnectionFactory(configuration);

            CreateDatabase();

            _repository = new TaskItemRepository(_factory);
        }

        [TearDown]
        public void TearDown()
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(_databasePath))
                File.Delete(_databasePath);
        }

        private void CreateDatabase()
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                CREATE TABLE TaskItems (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    ProjectId TEXT NOT NULL,
                    AssigneeId TEXT NOT NULL,
                    Description TEXT,
                    Status TEXT,
                    Priority TEXT, 
                    CreatedAt TEXT NOT NULL
                );
                """;

            command.ExecuteNonQuery();
        }

        [Test]
        public void Add_Task_CanBeRetrievedById()
        {
            // Arrange
            var task = TaskItem.Hydrate(
                id: Guid.NewGuid(),
                name: "Test Task",
                projectId: Guid.NewGuid(),
                assigneeId: Guid.NewGuid(),
                description: "Test description",
                status: "Pending", priority: "High",
                createdAt: DateTime.UtcNow
            );

            // Act
            _repository.Add(task);

            var result = _repository.GetById(task.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(task.Id));
            Assert.That(result.Name, Is.EqualTo(task.Name));
            Assert.That(result.ProjectId, Is.EqualTo(task.ProjectId));
            Assert.That(result.AssigneeId, Is.EqualTo(task.AssigneeId));
            Assert.That(result.Description, Is.EqualTo(task.Description));
            Assert.That(result.Status, Is.EqualTo(task.Status));
            Assert.That(result.Priority, Is.EqualTo(task.Priority));
        }

        [Test]
        public void GetById_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = _repository.GetById(id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByName_ReturnsTask()
        {
            // Arrange
            var task = TaskItem.Hydrate(
                id: Guid.NewGuid(),
                name: "Test Task",
                projectId: Guid.NewGuid(),
                assigneeId: Guid.NewGuid(),
                description: "Test description",
                status: "Pending", priority: "High",
                createdAt: DateTime.UtcNow
            );

            _repository.Add(task);

            // Act
            var result = _repository.GetByName(task.Name);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(task.Id));
            Assert.That(result.Name, Is.EqualTo(task.Name));
        }

        [Test]
        public void GetByName_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Act
            var result = _repository.GetByName("Does Not Exist");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByDescription_ReturnsTask()
        {
            // Arrange
            var task = TaskItem.Hydrate(
                id: Guid.NewGuid(),
                name: "Test Task",
                projectId: Guid.NewGuid(),
                assigneeId: Guid.NewGuid(),
                description: "Test description",
                status: "Pending", priority: "High",
                createdAt: DateTime.UtcNow
            );

            _repository.Add(task);

            // Act
            var result = _repository.GetByDescription(task.Description!);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(task.Id));
            Assert.That(result.Description, Is.EqualTo(task.Description));
        }

        [Test]
        public void GetByDescription_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Act
            var result = _repository.GetByDescription("Does Not Exist");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
