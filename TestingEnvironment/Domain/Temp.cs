using System;
using System.Collections.Generic;
using System.Text;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Domain
{
    [TestFixture]
    public class BuilderSanityTests
    {
        [Test]
        public void UserBuilder_Build_ReturnsValidUser()
        {
            var user = new UserBuilder().WithName("Carlos").Build();

            Assert.That(user.Name, Is.EqualTo("Carlos"));
            Assert.That(user.Active, Is.True);
        }

        [Test]
        public void ProjectBuilder_WithUsers_AddsAllUsers()
        {
            var u1 = Guid.NewGuid();
            var u2 = Guid.NewGuid();
            var project = new ProjectBuilder().WithUsers(u1, u2).Build();

            Assert.That(project.HasUser(u1), Is.True);
            Assert.That(project.HasUser(u2), Is.True);
        }

        [Test]
        public void TaskItemBuilder_ForProject_SetsProjectId()
        {
            var project = new ProjectBuilder().Build();
            var task = new TaskItemBuilder().ForProject(project).Build();

            Assert.That(task.ProjectId, Is.EqualTo(project.Id));
        }
    }
}