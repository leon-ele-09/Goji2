namespace GojiApi.Dtos
{
    public record CreateTaskRequest(
        string Name,
        Guid ProjectId,
        Guid AssigneeId,
        string? Description,
        string? Status,
        string? Priority);

    public record TaskResponse(
        Guid Id, string Name, Guid ProjectId, Guid AssigneeId,
        string? Description, string? Status, string? Priority, DateTime? CreatedAt)
    {
        public static TaskResponse From(Model.TaskItem.TaskItem task) =>
            new(task.Id, task.Name ?? string.Empty, task.ProjectId, task.AssigneeId,
                task.Description, task.Status, task.Priority, task.CreatedAt);
    }
}
