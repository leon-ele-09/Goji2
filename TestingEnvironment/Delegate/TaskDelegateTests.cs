using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using GojiApi.Delegate;
using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Delegate
{
    [TestFixture]
    public class TaskDelegateTests
    {
        private Mock<ITaskItem> _tasksMock = null!;
        private Mock<IProject> _projectsMock = null!;
        private Mock<IUser> _usersMock = null!;
        private TaskDelegate _delegate = null!;

        [SetUp]
        public void Setup()
        {
            _tasksMock = new Mock<ITaskItem>();
            _projectsMock = new Mock<IProject>();
            _usersMock = new Mock<IUser>();
            _delegate = new TaskDelegate(_tasksMock.Object, _projectsMock.Object, _usersMock.Object);
        }

        [Test]
        public void CreateTask_ValidData_ReturnsTaskWithExpectedFields()
        {
            var assignee = new UserBuilder().WithName("Shyam Sankar").Build();
            var project = new ProjectBuilder().WithUsers(assignee.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(assignee.Id)).Returns(assignee);

            var result = _delegate.CreateTask(
                "Desplegar Gotham", project.Id, assignee.Id,
                "Configurar pipeline", "To Do", "High"
            );

            Assert.That(result.Name, Is.EqualTo("Desplegar Gotham"));
            Assert.That(result.ProjectId, Is.EqualTo(project.Id));
            Assert.That(result.AssigneeId, Is.EqualTo(assignee.Id));
            Assert.That(result.Description, Is.EqualTo("Configurar pipeline"));
            Assert.That(result.Status, Is.EqualTo("To Do"));
            Assert.That(result.Priority, Is.EqualTo("High"));
        }

        [Test]
        public void CreateTask_ValidData_CallsRepositoryAddOnce()
        {
            var assignee = new UserBuilder().Build();
            var project = new ProjectBuilder().WithUsers(assignee.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(assignee.Id)).Returns(assignee);

            _delegate.CreateTask("Desplegar Gotham", project.Id, assignee.Id, null, null, null);

            _tasksMock.Verify(r => r.Add(It.IsAny<TaskItem>()), Times.Once);
        }

        [Test]
        public void CreateTask_WithoutOptionalFields_LeavesThemNull()
        {
            var assignee = new UserBuilder().Build();
            var project = new ProjectBuilder().WithUsers(assignee.Id).Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(assignee.Id)).Returns(assignee);

            var result = _delegate.CreateTask("Desplegar Gotham", project.Id, assignee.Id, null, null, null);

            Assert.That(result.Description, Is.Null);
            Assert.That(result.Status, Is.Null);
            Assert.That(result.Priority, Is.Null);
        }

        [Test]
        public void CreateTask_ProjectNotFound_ThrowsNotFoundException()
        {
            _projectsMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Project?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.CreateTask("Desplegar Gotham", Guid.NewGuid(), Guid.NewGuid(), null, null, null)
            );
        }

        [Test]
        public void CreateTask_AssigneeNotFound_ThrowsNotFoundException()
        {
            var project = new ProjectBuilder().Build();
            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

            Assert.Throws<NotFoundException>(
                () => _delegate.CreateTask("Desplegar Gotham", project.Id, Guid.NewGuid(), null, null, null)
            );
        }

        [Test]
        public void CreateTask_AssigneeNotInProject_ThrowsBusinessRuleException()
        {
            var assignee = new UserBuilder().Build();
            var project = new ProjectBuilder().Build(); // el assignee NO está en este proyecto

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(assignee.Id)).Returns(assignee);

            Assert.Throws<BusinessRuleException>(
                () => _delegate.CreateTask("Desplegar Gotham", project.Id, assignee.Id, null, null, null)
            );
        }

        [Test]
        public void CreateTask_AssigneeNotInProject_DoesNotCallAdd()
        {
            var assignee = new UserBuilder().Build();
            var project = new ProjectBuilder().Build();

            _projectsMock.Setup(r => r.GetById(project.Id)).Returns(project);
            _usersMock.Setup(r => r.GetById(assignee.Id)).Returns(assignee);

            Assert.Throws<BusinessRuleException>(
                () => _delegate.CreateTask("Desplegar Gotham", project.Id, assignee.Id, null, null, null)
            );

            _tasksMock.Verify(r => r.Add(It.IsAny<TaskItem>()), Times.Never);
        }

        [Test]
        public void GetTask_WhenExists_ReturnsTask()
        {
            var task = new TaskItemBuilder().WithName("Desplegar Gotham").Build();
            _tasksMock.Setup(r => r.GetById(task.Id)).Returns(task);

            var result = _delegate.GetTask(task.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Desplegar Gotham"));
        }

        [Test]
        public void GetTask_WhenNotFound_ReturnsNull()
        {
            _tasksMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((TaskItem?)null);

            var result = _delegate.GetTask(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetTasks_ByProjectId_ReturnsResultsFromRepository()
        {
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItemBuilder().ForProject(projectId).Build(),
                new TaskItemBuilder().ForProject(projectId).Build()
            };
            _tasksMock.Setup(r => r.GetByProjectId(projectId)).Returns(tasks);

            var result = _delegate.GetTasks(projectId, null);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetTasks_ByAssigneeId_ReturnsResultsFromRepository()
        {
            var assigneeId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItemBuilder().Build().WithAssignee(assigneeId)
            };
            _tasksMock.Setup(r => r.GetByAssigneeId(assigneeId)).Returns(tasks);

            var result = _delegate.GetTasks(null, assigneeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetTasks_NeitherProvided_ThrowsBusinessRuleException()
        {
            Assert.Throws<BusinessRuleException>(
                () => _delegate.GetTasks(null, null)
            );
        }

        [Test]
        public void GetTasks_BothProvided_FiltersByAssigneeWithinProjectResults()
        {
            var projectId = Guid.NewGuid();
            var matchingAssigneeId = Guid.NewGuid();
            var otherAssigneeId = Guid.NewGuid();

            var tasks = new List<TaskItem>
            {
                new TaskItemBuilder().ForProject(projectId).Build().WithAssignee(matchingAssigneeId),
                new TaskItemBuilder().ForProject(projectId).Build().WithAssignee(otherAssigneeId)
            };
            _tasksMock.Setup(r => r.GetByProjectId(projectId)).Returns(tasks);

            var result = _delegate.GetTasks(projectId, matchingAssigneeId);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().AssigneeId, Is.EqualTo(matchingAssigneeId));
        }
    }
}
