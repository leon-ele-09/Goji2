using System;
using System.Collections.Generic;
using System.Text;
using GojiApi.Model.User;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Domain
{
    [TestFixture]
    public class UserTests
    {
        public void Create_ValidName_SetsExpectedDefaults()
        {
            var user = User.Create("Ana");

            Assert.That(user.Name, Is.EqualTo("Ana"));
            Assert.That(user.Active, Is.True);
            Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(user.CreatedAt, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Create_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            Assert.Throws<ArgumentException>(() => User.Create(invalidName!));
        }

        [Test]
        public void WithName_ValidName_UpdatesName()
        {
            var user = new UserBuilder().Build();

            user.WithName("Carlos");

            Assert.That(user.Name, Is.EqualTo("Carlos"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void WithName_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            var user = new UserBuilder().Build();

            Assert.Throws<ArgumentException>(() => user.WithName(invalidName!));
        }

        [Test]
        public void WithName_ReturnsSameUserInstance_ForChaining()
        {
            var user = new UserBuilder().Build();

            var result = user.WithName("Carlos");

            Assert.That(result, Is.SameAs(user));
        }

        [Test]
        public void WithEmail_ValidEmail_SetsEmail()
        {
            var user = new UserBuilder().Build();

            user.WithEmail("ana@test.com");

            Assert.That(user.Email, Is.EqualTo("ana@test.com"));
        }

        [TestCase("sin-arroba.com")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("Hola_Brodi")]
        public void WithEmail_InvalidEmail_ThrowsArgumentException(string? invalidEmail)
        {
            var user = new UserBuilder().Build();

            Assert.Throws<ArgumentException>(() => user.WithEmail(invalidEmail!));
        }

        [Test]
        public void WithEmail_WhenThrows_DoesNotChangeEmail()
        {
            var user = new UserBuilder().WithEmail("original@test.com").Build();

            Assert.Throws<ArgumentException>(() => user.WithEmail("invalido"));

            Assert.That(user.Email, Is.EqualTo("original@test.com"));
        }

        [Test]
        public void WithEmail_ReturnsSameUserInstance_ForChaining()
        {
            var user = new UserBuilder().Build();

            var result = user.WithEmail("carlos@palantir.com");

            Assert.That(result, Is.SameAs(user));
        }

        [Test]
        public void Activate_SetsActiveTrue()
        {
            var user = new UserBuilder().Inactive().Build();

            user.Activate();

            Assert.That(user.Active, Is.True);
        }

        [Test]
        public void Activate_WhenAlreadyActive_StaysActive()
        {
            var user = new UserBuilder().Build();

            user.Activate();

            Assert.That(user.Active, Is.True);
        }

        [Test]
        public void Deactivate_SetsActiveFalse()
        {
            var user = new UserBuilder().Build();

            user.Deactivate();

            Assert.That(user.Active, Is.False);
        }

        [Test]
        public void Deactivate_WhenAlreadyInactive_StaysInactive()
        {
            var user = new UserBuilder().Inactive().Build();

            user.Deactivate();

            Assert.That(user.Active, Is.False);
        }
    }
}