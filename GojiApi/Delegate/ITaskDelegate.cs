using GojiApi.Model.TaskItem;

namespace GojiApi.Delegate
{
    public interface ITaskDelegate
    {
        
        TaskItem CreateTask(string name, Guid projectId, Guid assigneeId, string? description, string? status, string? priority);

        TaskItem? GetTask(Guid id);

        
        IEnumerable<TaskItem> GetTasks(Guid? projectId, Guid? assigneeId);
    }
}
