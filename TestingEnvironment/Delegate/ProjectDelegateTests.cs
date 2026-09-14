using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using GojiApi.Delegate;
using GojiApi.Model.Project;
using GojiApi.Model.User;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Delegate
{
    [TestFixture]
    public class ProjectDelegateTests
    {
        private Mock<IProject> _projectsMock = null!;
        private Mock<IUser> _usersMock = null!;
        private ProjectDelegate _delegate = null!;

        [SetUp]
        public void Setup()
        {
            _projectsMock = new Mock<IProject>();
            _usersMock = new Mock<IUser>();
            _delegate = new ProjectDelegate(_projectsMock.Object, _usersMock.Object);
        }

        [Test]
        public void CreateProject_ValidData_ReturnsProjectWithOwnerAdded()
        {
            var owner = new UserBuilder().WithName("Alex Karp").Build();
            _usersMock.Setup(r => r.GetById(owner.Id)).Returns(owner);

            var result = _delegate.CreateProject("Palantir OS", "PLTR", owner.Id);

            Assert.That(result.Name, Is.EqualTo("Palantir OS"));
            Assert.That(result.BusinessKey, Is.EqualTo("PLTR"));
            Assert.That(result.HasUser(owner.Id), Is.True);
        }

        [Test]
        public void CreateProject_ValidData_CallsRepositoryAddOnce()
        {
            var owner = new UserBuilder().Build();
            _usersMock.Setup(r => r.GetById(owner.Id)).Returns(owner);

            _delegate.CreateProject("Palantir OS", "PLTR", owner.Id);

            _projectsMock.Verify(r => r.Add(It.IsAny<Project>()), Times.Once);
        }

        [Test]
        public void CreateProject_OwnerNotFound_ThrowsNotFoundException()
        {
            _usersMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.CreateProject("Palantir OS", "PLTR", Guid.NewGuid())
            );
        }

        [Test]
        public void CreateProject_OwnerNotFound_DoesNotCallAdd()
        {
            _usersMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.CreateProject("Palantir OS", "PLTR", Guid.NewGuid())
            );

            _projectsMock.Verify(r => r.Add(It.IsAny<Project>()), Times.Never);
        }

        [Test]
        public void CreateProject_WithoutBusinessKey_CreatesProjectWithNullBusinessKey()
        {
            var owner = new UserBuilder().Build();
            _usersMock.Setup(r => r.GetById(owner.Id)).Returns(owner);

            var result = _delegate.CreateProject("Palantir OS", null, owner.Id);

            Assert.That(result.Name, Is.EqualTo("Palantir OS"));
            Assert.That(result.BusinessKey, Is.Null);
        }

        [Test]
        public void GetProject_WhenExists_ReturnsProject()
        {
            var project = new ProjectBuilder().WithName("Gotham").Build();
            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);

            var result = _delegate.GetProject(project.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Gotham"));
        }

        [Test]
        public void GetProject_WhenNotFound_ReturnsNull()
        {
            _projectsMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Project?)null);

            var result = _delegate.GetProject(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        // ============================================================
        // AddUserToProject
        // ============================================================

        [Test]
        public void AddUserToProject_ValidRequest_AddsUserAndReturnsProject()
        {
            var requester = new UserBuilder().WithName("Alex Karp").Build();
            var userToAdd = new UserBuilder().WithName("Shyam Sankar").Build();
            var project = new ProjectBuilder().WithUsers(requester.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(userToAdd.Id)).Returns(userToAdd);

            var result = _delegate.AddUserToProject(project.Id, requester.Id, userToAdd.Id);

            Assert.That(result.HasUser(userToAdd.Id), Is.True);
        }

        [Test]
        public void AddUserToProject_ValidRequest_CallsRepositoryUpdateOnce()
        {
            var requester = new UserBuilder().Build();
            var userToAdd = new UserBuilder().Build();
            var project = new ProjectBuilder().WithUsers(requester.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(userToAdd.Id)).Returns(userToAdd);

            _delegate.AddUserToProject(project.Id, requester.Id, userToAdd.Id);

            _projectsMock.Verify(r => r.Update(It.IsAny<Project>()), Times.Once);
        }

        [Test]
        public void AddUserToProject_ProjectNotFound_ThrowsNotFoundException()
        {
            _projectsMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Project?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.AddUserToProject(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
            );
        }

        [Test]
        public void AddUserToProject_RequesterNotInProject_ThrowsForbiddenException()
        {
            var project = new ProjectBuilder().Build(); // sin ningún usuario dentro
            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);

            Assert.Throws<ForbiddenException>(
                () => _delegate.AddUserToProject(project.Id, Guid.NewGuid(), Guid.NewGuid())
            );
        }

        [Test]
        public void AddUserToProject_UserToAddNotFound_ThrowsNotFoundException()
        {
            var requester = new UserBuilder().Build();
            var project = new ProjectBuilder().WithUsers(requester.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.AddUserToProject(project.Id, requester.Id, Guid.NewGuid())
            );
        }
    }
}
