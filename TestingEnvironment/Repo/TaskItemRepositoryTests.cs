using GojiApi.Model.TaskItem;
using GojiApi.Repository;
using NUnit.Framework;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Repo
{
    [TestFixture]
    public class TaskItemRepositoryTests
    {
        private TaskItemRepository _repository = null!;
        private PostgresTestDatabase _database = null!;

        [SetUp]
        public void Setup()
        {
            _database = new PostgresTestDatabase(
                """
                CREATE TABLE TaskItems (
                    Id UUID PRIMARY KEY,
                    Name TEXT NOT NULL,
                    ProjectId UUID NOT NULL,
                    AssigneeId UUID NOT NULL,
                    Description TEXT,
                    Status TEXT,
                    Priority TEXT,
                    CreatedAt TIMESTAMPTZ NOT NULL
                );
                """);

            _repository = new TaskItemRepository(_database.Factory);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Dispose();
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