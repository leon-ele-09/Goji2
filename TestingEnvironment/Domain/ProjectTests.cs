using GojiApi.Model.Project;
using GojiApi.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Domain
{
    [TestFixture]
    public class ProjectTests
    {
        [Test]
        public void Create_SetsIdAndCreatedAt()
        {
            var project = Project.Create();

            Assert.That(project.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(project.CreatedAt, Is.Not.Null);
        }

       [Test]
        public void WithName_ValidName_SetsName()
        {
            var project = new ProjectBuilder().Build();

            project.WithName("Palantir OS");

            Assert.That(project.Name, Is.EqualTo("Palantir OS"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void WithName_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            var project = new ProjectBuilder().Build();

            Assert.Throws<ArgumentException>(() => project.WithName(invalidName!));
        }

        [Test]
        public void WithName_ReturnsSameProjectInstance_ForChaining()
        {
            var project = new ProjectBuilder().Build();

            var result = project.WithName("Palantir OS");

            Assert.That(result, Is.SameAs(project));
        }

       [Test]
        public void WithBusinessKey_ValidKey_SetsBusinessKey()
        {
            var project = new ProjectBuilder().Build();

            project.WithBusinessKey("PLTR");

            Assert.That(project.BusinessKey, Is.EqualTo("PLTR"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void WithBusinessKey_InvalidKey_ThrowsArgumentException(string? invalidKey)
        {
            var project = new ProjectBuilder().Build();

            Assert.Throws<ArgumentException>(() => project.WithBusinessKey(invalidKey!));
        }

        [Test]
        public void WithBusinessKey_ReturnsSameProjectInstance_ForChaining()
        {
            var project = new ProjectBuilder().Build();

            var result = project.WithBusinessKey("PLTR");

            Assert.That(result, Is.SameAs(project));
        }

        [Test]
        public void HasUser_UserExists_ReturnsTrue()
        {
            var alexId = Guid.NewGuid();
            var project = new ProjectBuilder().WithUsers(alexId).Build();

            Assert.That(project.HasUser(alexId), Is.True);
        }

        [Test]
        public void HasUser_UserDoesNotExist_ReturnsFalse()
        {
            var project = new ProjectBuilder().Build();

            Assert.That(project.HasUser(Guid.NewGuid()), Is.False);
        }

        [Test]
        public void AddUser_NewUser_AddsToList()
        {
            var project = new ProjectBuilder().Build();
            var peterId = Guid.NewGuid();

            project.AddUser(peterId);

            Assert.That(project.HasUser(peterId), Is.True);
        }

        [Test]
        public void AddUser_DuplicateUser_DoesNotAddTwice()
        {
            var project = new ProjectBuilder().Build();
            var peterId = Guid.NewGuid();

            project.AddUser(peterId);
            project.AddUser(peterId);

            Assert.That(project.UserIds.Count(id => id == peterId), Is.EqualTo(1));
        }

        [Test]
        public void AddUser_ReturnsSameProjectInstance_ForChaining()
        {
            var project = new ProjectBuilder().Build();

            var result = project.AddUser(Guid.NewGuid());

            Assert.That(result, Is.SameAs(project));
        }

        [Test]
        public void RemoveUser_ExistingUser_RemovesFromList()
        {
            var joeId = Guid.NewGuid();
            var project = new ProjectBuilder().WithUsers(joeId).Build();

            project.RemoveUser(joeId);

            Assert.That(project.HasUser(joeId), Is.False);
        }

        [Test]
        public void RemoveUser_NonExistentUser_DoesNotThrow()
        {
            var project = new ProjectBuilder().Build();

            Assert.DoesNotThrow(() => project.RemoveUser(Guid.NewGuid()));
        }

        [Test]
        public void RemoveUser_ReturnsSameProjectInstance_ForChaining()
        {
            var project = new ProjectBuilder().Build();

            var result = project.RemoveUser(Guid.NewGuid());

            Assert.That(result, Is.SameAs(project));
        }
    }
}
