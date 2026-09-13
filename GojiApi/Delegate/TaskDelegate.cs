using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;

namespace GojiApi.Delegate
{
    public class TaskDelegate : ITaskDelegate
    {
        private readonly ITaskItem _tasks;
        private readonly IProject _projects;
        private readonly IUser _users;

        public TaskDelegate(ITaskItem tasks, IProject projects, IUser users)
        {
            _tasks = tasks;
            _projects = projects;
            _users = users;
        }

        public TaskItem CreateTask(string name, Guid projectId, Guid assigneeId, string? description, string? status, string? priority)
        {
            var project = _projects.GetById(projectId)
                ?? throw new NotFoundException($"Proyeco '{projectId}' no existe.");

            var assignee = _users.GetById(assigneeId)
                ?? throw new NotFoundException($"Usuario '{assigneeId}' no existe.");

            
            if (!project.HasUser(assignee.Id))
            {
                throw new BusinessRuleException("Usuario no esta en proyecto.");
            }

            
            var task = TaskItem.Create(name)
                .WithProject(project.Id)
                .WithAssignee(assignee.Id);

            if (!string.IsNullOrWhiteSpace(description)) task = task.WithDescription(description);
            if (!string.IsNullOrWhiteSpace(status)) task = task.WithStatus(status);
            if (!string.IsNullOrWhiteSpace(priority)) task = task.WithPriority(priority);

            _tasks.Add(task);
            return task;
        }

        public TaskItem? GetTask(Guid id) => _tasks.GetById(id);

        public IEnumerable<TaskItem> GetTasks(Guid? projectId, Guid? assigneeId)
        {
            IEnumerable<TaskItem> results;

            if (projectId is not null)
            {
                results = _tasks.GetByProjectId(projectId.Value);
            }
            else if (assigneeId is not null)
            {
                results = _tasks.GetByAssigneeId(assigneeId.Value);
            }
            else
            {
                throw new BusinessRuleException("Falta proyecto y/o usuario.");
            }

            if (projectId is not null && assigneeId is not null)
            {
                results = results.Where(t => t.AssigneeId == assigneeId.Value);
            }

            return results;
        }
    }
}
