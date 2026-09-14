using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using GojiApi.Delegate;
using GojiApi.Model.User;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Delegate
{
    [TestFixture]
    public class UserDelegateTests
    {
        private Mock<IUser> _usersMock = null!;
        private UserDelegate _delegate = null!;

        [SetUp]
        public void Setup()
        {
            _usersMock = new Mock<IUser>();
            _delegate = new UserDelegate(_usersMock.Object);
        }

        [Test]
        public void RegisterUser_ValidData_ReturnsUserWithCorrectFields()
        {
            _usersMock.Setup(r => r.GetByName(It.IsAny<string>())).Returns((User?)null);
            _usersMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User?)null);

            var result = _delegate.RegisterUser("Alex Karp", "alex@palantir.com");

            Assert.That(result.Name, Is.EqualTo("Alex Karp"));
            Assert.That(result.Email, Is.EqualTo("alex@palantir.com"));
        }

        [Test]
        public void RegisterUser_ValidData_CallsRepositoryAddOnce()
        {
            _usersMock.Setup(r => r.GetByName(It.IsAny<string>())).Returns((User?)null);
            _usersMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User?)null);

            _delegate.RegisterUser("Alex Karp", "alex@palantir.com");

            _usersMock.Verify(
                r => r.Add(It.Is<User>(u => u.Name == "Alex Karp" && u.Email == "alex@palantir.com")),
                Times.Once
            );
        }

        [Test]
        public void RegisterUser_DuplicateName_ThrowsConflictException()
        {
            var existingUser = new UserBuilder().WithName("Alex Karp").Build();
            _usersMock.Setup(r => r.GetByName("Alex Karp")).Returns(existingUser);

            Assert.Throws<ConflictException>(
                () => _delegate.RegisterUser("Alex Karp", "otro@palantir.com")
            );
        }

        [Test]
        public void RegisterUser_DuplicateEmail_ThrowsConflictException()
        {
            var existingUser = new UserBuilder().WithEmail("alex@palantir.com").Build();
            _usersMock.Setup(r => r.GetByName(It.IsAny<string>())).Returns((User?)null);
            _usersMock.Setup(r => r.GetByEmail("alex@palantir.com")).Returns(existingUser);

            Assert.Throws<ConflictException>(
                () => _delegate.RegisterUser("Otro Nombre", "alex@palantir.com")
            );
        }

        [Test]
        public void RegisterUser_DuplicateName_DoesNotCallAdd()
        {
            var existingUser = new UserBuilder().WithName("Alex Karp").Build();
            _usersMock.Setup(r => r.GetByName("Alex Karp")).Returns(existingUser);

            Assert.Throws<ConflictException>(
                () => _delegate.RegisterUser("Alex Karp", "otro@palantir.com")
            );

            _usersMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void GetUser_WhenExists_ReturnsUser()
        {
            var user = new UserBuilder().WithName("Peter Thiel").Build();
            _usersMock.Setup(r => r.GetById(user.Id)).Returns(user);

            var result = _delegate.GetUser(user.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Peter Thiel"));
        }

        [Test]
        public void GetUser_WhenNotFound_ReturnsNull()
        {
            _usersMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

            var result = _delegate.GetUser(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }
    }
}
