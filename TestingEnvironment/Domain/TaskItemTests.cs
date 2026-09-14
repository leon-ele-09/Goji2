using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using TestingEnvironment.TestCommon;

namespace TestingEnvironment.Domain
{
    [TestFixture]
    public class TaskItemTests
    {
        [Test]
        public void Create_ValidName_SetsExpectedDefaults()
        {
            var task = TaskItem.Create("Desplegar Gotham");

            Assert.That(task.Name, Is.EqualTo("Desplegar Gotham"));
            Assert.That(task.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(task.CreatedAt, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Create_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            Assert.Throws<ArgumentException>(() => TaskItem.Create(invalidName!));
        }

        [Test]
        public void WithName_ValidName_UpdatesName()
        {
            var task = new TaskItemBuilder().Build();

            task.WithName("Integrar Foundry");

            Assert.That(task.Name, Is.EqualTo("Integrar Foundry"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void WithName_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            var task = new TaskItemBuilder().Build();

            Assert.Throws<ArgumentException>(() => task.WithName(invalidName!));
        }

        [Test]
        public void WithName_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithName("Integrar Foundry");

            Assert.That(result, Is.SameAs(task));
        }

        [Test]
        public void WithProject_SetsProjectId()
        {
            var task = new TaskItemBuilder().Build();
            var project = new ProjectBuilder().WithName("Palantir OS").Build();

            task.WithProject(project.Id);

            Assert.That(task.ProjectId, Is.EqualTo(project.Id));
        }

        [Test]
        public void WithProject_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithProject(Guid.NewGuid());

            Assert.That(result, Is.SameAs(task));
        }

        [Test]
        public void WithAssignee_SetsAssigneeId()
        {
            var task = new TaskItemBuilder().Build();
            var alex = new UserBuilder().WithName("Alex Karp").Build();

            task.WithAssignee(alex.Id);

            Assert.That(task.AssigneeId, Is.EqualTo(alex.Id));
        }

        [Test]
        public void WithAssignee_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithAssignee(Guid.NewGuid());

            Assert.That(result, Is.SameAs(task));
        }

        [Test]
        public void WithDescription_ValidDescription_SetsDescription()
        {
            var task = new TaskItemBuilder().Build();

            task.WithDescription("Configurar el pipeline de datos para Gotham");

            Assert.That(task.Description, Is.EqualTo("Configurar el pipeline de datos para Gotham"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void WithDescription_InvalidDescription_ThrowsArgumentException(string? invalidDescription)
        {
            var task = new TaskItemBuilder().Build();

            Assert.Throws<ArgumentException>(() => task.WithDescription(invalidDescription!));
        }

        [Test]
        public void WithDescription_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithDescription("Configurar el pipeline de datos para Gotham");

            Assert.That(result, Is.SameAs(task));
        }

        [TestCase("To Do")]
        [TestCase("In Progress")]
        [TestCase("In Review")]
        [TestCase("Done")]
        public void WithStatus_ValidStatus_SetsStatus(string validStatus)
        {
            var task = new TaskItemBuilder().Build();

            task.WithStatus(validStatus);

            Assert.That(task.Status, Is.EqualTo(validStatus));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("Terminado")]
        public void WithStatus_InvalidStatus_ThrowsArgumentException(string? invalidStatus)
        {
            var task = new TaskItemBuilder().Build();

            Assert.Throws<ArgumentException>(() => task.WithStatus(invalidStatus!));
        }

        [Test]
        public void WithStatus_CaseInsensitive_StillSetsStatus()
        {
            var task = new TaskItemBuilder().Build();

            task.WithStatus("in review");

            Assert.That(task.Status, Is.EqualTo("in review"));
        }

        [Test]
        public void WithStatus_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithStatus("Done");

            Assert.That(result, Is.SameAs(task));
        }

        [TestCase("Lowest")]
        [TestCase("Low")]
        [TestCase("Medium")]
        [TestCase("High")]
        [TestCase("Highest")]
        public void WithPriority_ValidPriority_SetsPriority(string validPriority)
        {
            var task = new TaskItemBuilder().Build();

            task.WithPriority(validPriority);

            Assert.That(task.Priority, Is.EqualTo(validPriority));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("Urgente")]
        public void WithPriority_InvalidPriority_ThrowsArgumentException(string? invalidPriority)
        {
            var task = new TaskItemBuilder().Build();

            Assert.Throws<ArgumentException>(() => task.WithPriority(invalidPriority!));
        }

        [Test]
        public void WithPriority_CaseInsensitive_StillSetsPriority()
        {
            var task = new TaskItemBuilder().Build();

            task.WithPriority("high");

            Assert.That(task.Priority, Is.EqualTo("high"));
        }

        [Test]
        public void WithPriority_ReturnsSameTaskItemInstance_ForChaining()
        {
            var task = new TaskItemBuilder().Build();

            var result = task.WithPriority("Highest");

            Assert.That(result, Is.SameAs(task));
        }
    }
}

