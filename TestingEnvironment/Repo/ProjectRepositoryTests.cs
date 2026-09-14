using GojiApi.Data;
using GojiApi.Model.Project;
using GojiApi.Repository;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace TestingEnvironment.Repo
{
    [TestFixture]
    public class ProjectRepositoryTests
    {
        private ProjectRepository _repository = null!;
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

            _repository = new ProjectRepository(_factory);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_databasePath))
                File.Delete(_databasePath);
        }

        private void CreateDatabase()
        {
            using var connection = _factory.CreateOpenConnection();
            using var command = connection.CreateCommand();

            command.CommandText =
                """
                CREATE TABLE Projects (
                    Id TEXT PRIMARY KEY,
                    BusinessKey TEXT,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE ProjectUsers (
                    ProjectId TEXT NOT NULL,
                    UserId TEXT NOT NULL,
                    PRIMARY KEY (ProjectId, UserId)
                );
                """;

            command.ExecuteNonQuery();
        }

        [Test]
        public void Add_Project_CanBeRetrievedById()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            var project = Project.Hydrate(
                id: projectId,
                name: "Test Project",
                businessKey: "TEST-001",
                userIds: new List<Guid>(),
                createdAt: DateTime.Now
            );

            // Act
            _repository.Add(project);

            var result = _repository.GetById(projectId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(projectId));
            Assert.That(result.Name, Is.EqualTo("Test Project"));
            Assert.That(result.BusinessKey, Is.EqualTo("TEST-001"));
        }

        [Test]
        public void GetById_ReturnsNull_WhenProjectDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = _repository.GetById(id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByName_ReturnsProject()
        {
            // Arrange
            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "My Project",
                businessKey: "PROJECT-001",
                userIds: new List<Guid>(),
                createdAt: DateTime.Now
            );

            _repository.Add(project);

            // Act
            var result = _repository.GetByName("My Project");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(project.Id));
            Assert.That(result.Name, Is.EqualTo("My Project"));
        }

        [Test]
        public void GetByName_ReturnsNull_WhenProjectDoesNotExist()
        {
            // Act
            var result = _repository.GetByName("Does Not Exist");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByBusinessKey_ReturnsProject()
        {
            // Arrange
            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "My Project",
                businessKey: "BUS-123",
                userIds: new List<Guid>(),
                createdAt: DateTime.Now
            );

            _repository.Add(project);

            // Act
            var result = _repository.GetByBusinessKey("BUS-123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(project.Id));
            Assert.That(result.BusinessKey, Is.EqualTo("BUS-123"));
        }

        [Test]
        public void Update_Project_ChangesNameAndBusinessKey()
        {
            // Arrange
            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "Original Name",
                businessKey: "ORIGINAL",
                userIds: new List<Guid>(),
                createdAt: DateTime.Now
            );

            _repository.Add(project);

            project.Name = "Updated Name";
            project.BusinessKey = "UPDATED";

            // Act
            _repository.Update(project);

            var result = _repository.GetById(project.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.BusinessKey, Is.EqualTo("UPDATED"));
        }

        [Test]
        public void Delete_Project_RemovesProject()
        {
            // Arrange
            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "Project To Delete",
                businessKey: "DELETE-001",
                userIds: new List<Guid>(),
                createdAt: DateTime.Now
            );

            _repository.Add(project);

            // Act
            _repository.Delete(project);

            var result = _repository.GetById(project.Id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Add_Project_SavesUserIds()
        {
            // Arrange
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "Project With Users",
                businessKey: "USERS-001",
                userIds: new List<Guid>
                {
                    user1,
                    user2
                },
                createdAt: DateTime.Now
            );

            // Act
            _repository.Add(project);

            var result = _repository.GetById(project.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserIds, Has.Count.EqualTo(2));
            Assert.That(result.UserIds, Does.Contain(user1));
            Assert.That(result.UserIds, Does.Contain(user2));
        }

        [Test]
        public void Update_Project_ReplacesUserIds()
        {
            // Arrange
            var oldUser = Guid.NewGuid();
            var newUser = Guid.NewGuid();

            var project = Project.Hydrate(
                id: Guid.NewGuid(),
                name: "Project",
                businessKey: "UPDATE-USERS",
                userIds: new List<Guid>
                {
                    oldUser
                },
                createdAt: DateTime.Now
            );

            _repository.Add(project);

            project.UserIds.Clear();
            project.UserIds.Add(newUser);

            // Act
            _repository.Update(project);

            var result = _repository.GetById(project.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserIds, Has.Count.EqualTo(1));
            Assert.That(result.UserIds, Does.Contain(newUser));
            Assert.That(result.UserIds, Does.Not.Contain(oldUser));
        }
    }
}