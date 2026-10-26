using GojiApi.Model.User;
using GojiApi.Repository;
using NUnit.Framework;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Repo
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository _repository = null!;
        private PostgresTestDatabase _database = null!;

        [SetUp]
        public void Setup()
        {
            _database = new PostgresTestDatabase(
                """
                CREATE TABLE Users (
                    Id UUID PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Active BOOLEAN NOT NULL,
                    CreatedAt TIMESTAMPTZ NOT NULL
                );
                """);

            _repository = new UserRepository(_database.Factory);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Dispose();
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
                createdAt: DateTime.UtcNow
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
            // Postgres guarda microsegundos, por eso la tolerancia
            Assert.That(result.CreatedAt, Is.EqualTo(user.CreatedAt).Within(TimeSpan.FromMilliseconds(1)));
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
                createdAt: (DateTime)user.CreatedAt!
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