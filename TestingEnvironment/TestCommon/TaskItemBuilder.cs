using System;
using System.Collections.Generic;
using System.Text;
using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;

namespace TestingEnvironment.TestCommon
{
    public class TaskItemBuilder
    {
        private string _name = "Tarea de Prueba";
        private Guid _projectId = Guid.NewGuid();
        private string _status = "To Do";
        private string _priority = "Medium";

        public TaskItemBuilder ForProject(Project project)
        {
            _projectId = project.Id;
            return this;
        }

        public TaskItemBuilder ForProject(Guid projectId)
        {
            _projectId = projectId;
            return this;
        }

        public TaskItemBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public TaskItemBuilder WithStatus(string status)
        {
            _status = status;
            return this;
        }

        public TaskItemBuilder WithPriority(string priority)
        {
            _priority = priority;
            return this;
        }

        public TaskItem Build()
        {
            return TaskItem.Create(_name)
                .WithProject(_projectId)
                .WithStatus(_status)
                .WithPriority(_priority);
        }
    }
}