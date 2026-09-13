namespace GojiApi.Model.TaskItem
{
    public interface ITaskItem
    {
        void Add(TaskItem taskItem);
        TaskItem GetById(Guid id);
        TaskItem GetByName(string name);
        TaskItem GetByDescription(string description);
        TaskItem GetByStatus(string status);
        TaskItem GetByPriority(string priority);
        void Update(TaskItem taskItem);
        void Delete(TaskItem taskItem);
    }
}