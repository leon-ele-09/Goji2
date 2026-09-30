using GojiApi.Data;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;
using GojiApi.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace TestingEnvironment.Repo
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository _repository = null!;
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

            _repository = new UserRepository(_factory);
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
                CREATE TABLE Users (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL, 
                    Active BOOL NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
                """;

            command.ExecuteNonQuery();
        }

        [Test]
        public void AddUser_CanBeRetrievedById()
        {
            // Arrange
            var user = User.Hydrate(
                id: Guid.NewGuid(),
                name: "Test User",
                email: "Test email",
                active: true,
                createdAt: DateTime.Now
            );

            // Act
            _repository.Add(user);

            var result = _repository.GetById(user.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Active, Is.EqualTo(user.Active));
            Assert.That(result.CreatedAt, Is.EqualTo(user.CreatedAt));
        }

        [Test]
        public void GetById_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = _repository.GetById(id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByName_ReturnTask()
        {
            // Arrange
            var user = User.Hydrate(
                id: Guid.NewGuid(),
                name: "Test User",
                email: "Test email",
                active: true,
                createdAt: DateTime.UtcNow
            );

            // Act
            _repository.Add(user);

            var result = _repository.GetByName(user.Name);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
        }

        [Test]
        public void GetByName_ReturnNull_WhenUserDoesntExist()
        {
            // Act
            var result = _repository.GetByName("Does Not Exist");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByEmail_ReturnTask()
        {
            // Arrange
            var user = User.Hydrate(
                id: Guid.NewGuid(),
                name: "Test User",
                email: "Test email",
                active: true,
                createdAt: DateTime.UtcNow
            );

            // Act
            _repository.Add(user);

            var result = _repository.GetByEmail(user.Email);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public void GetByEmail_ReturnNull_WhenUserDoesntExist()
        {
            // Act
            var result = _repository.GetByEmail("Does Not Exist");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateUser_ChangesUserData()
        {
            // Arrange
            var user = User.Hydrate(
                id: Guid.NewGuid(),
                name: "Original Name",
                email: "original@email.com",
                active: true,
                createdAt: DateTime.UtcNow
            );

            _repository.Add(user);

            var updatedUser = User.Hydrate(
                id: user.Id,
                name: "Updated Name",
                email: "updated@email.com",
                active: false,
                createdAt: (DateTime)user.CreatedAt
            );

            // Act
            _repository.Update(updatedUser);

            var result = _repository.GetById(user.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.Email, Is.EqualTo("updated@email.com"));
            Assert.That(result.Active, Is.False);
        }

        [Test]
        public void DeleteUser_RemovesUser()
        {
            // Arrange
            var user = User.Hydrate(
                id: Guid.NewGuid(),
                name: "User To Delete",
                email: "delete@email.com",
                active: true,
                createdAt: DateTime.UtcNow
            );

            _repository.Add(user);

            // Act
            _repository.Delete(user);

            var result = _repository.GetById(user.Id);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
