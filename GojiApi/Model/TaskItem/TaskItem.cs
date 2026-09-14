namespace GojiApi.Model.TaskItem
{
    public class TaskItem
    {
        public Guid Id { get; private set; }
        public string? Name { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AssigneeId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? CreatedAt { get; private set; }

        public static TaskItem Create(string name) => new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        }.WithName(name);


        internal static TaskItem Hydrate(
            Guid id, string name, Guid projectId, Guid assigneeId,
            string? description, string? status, string? priority, DateTime createdAt) =>
            new TaskItem
            {
                Id = id,
                Name = name,
                ProjectId = projectId,
                AssigneeId = assigneeId,
                Description = description,
                Status = status,
                Priority = priority,
                CreatedAt = createdAt
            };

        public TaskItem WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("Invalid Name"); }

            Name = name;
            return this;
        }
        
        public TaskItem WithProject(Guid Id)
        {
            ProjectId = Id;
            return this;
        }
        
        public TaskItem WithAssignee(Guid Id)
        {
            AssigneeId = Id;
            return this;
        }

        

        public TaskItem WithDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) { throw new ArgumentException("Invalid Description"); }

            Description = description;
            return this;
        }

        private readonly string[] allowedStatus = { "To Do", "In Progress", "In Review", "Done" };

        public TaskItem WithStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status) || !allowedStatus.Contains(status, StringComparer.OrdinalIgnoreCase)) { throw new ArgumentException("Invalid Status"); }

            Status = status;
            return this;
        }

        private readonly string[] allowedPriority = { "Lowest", "Low", "Medium", "High", "Highest" };

        public TaskItem WithPriority(string priority)
        {
            if (string.IsNullOrWhiteSpace(priority) || !allowedPriority.Contains(priority, StringComparer.OrdinalIgnoreCase)) { throw new ArgumentException("Invalid Priority"); }

            Priority = priority;
            return this;
        }


    }
}
