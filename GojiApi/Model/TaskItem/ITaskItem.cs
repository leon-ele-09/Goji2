namespace GojiApi.Model.TaskItem
{
    public interface ITaskItem
    {
        void Add(TaskItem taskItem);
        TaskItem? GetById(Guid id);
        TaskItem? GetByName(string name);
        TaskItem? GetByDescription(string description);
        IEnumerable<TaskItem> GetByStatus(string status);
        IEnumerable<TaskItem> GetByPriority(string priority);
        IEnumerable<TaskItem> GetByProjectId(Guid projectId);
        IEnumerable<TaskItem> GetByAssigneeId(Guid assigneeId);
        void Update(TaskItem taskItem);
        void Delete(TaskItem taskItem);
    }
}