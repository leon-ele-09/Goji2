namespace GojiApi.Model.TaskItem
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AssigneeId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? CreatedAt { get; private set; }

        public static TaskItem Create(string name) => new TaskItem
        {
            Name = name,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        public TaskItem WithAssignee(Guid Id)
        {
            AssigneeId = Id;
            return this;
        }

        public TaskItem WithName(string name)
        {
            Name = name; 
            return this;
        }

        public TaskItem WithDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) { throw new ArgumentException("Invalid Email"); }

            Description = description;
            return this;
        }

        private readonly string[] allowedStatus = { "To Do", "In Progress", "In Review", "Done" };

        public TaskItem WithStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status) || !allowedStatus.Contains(status, StringComparer.OrdinalIgnoreCase)) { throw new ArgumentException("Invalid Status"); }

            Description = status;
            return this;
        }

        private readonly string[] allowedPriority = { "Lowest", "Low", "Medium", "High", "Highest" };

        public TaskItem WithPriority(string priority)
        {
            if (string.IsNullOrWhiteSpace(priority) || !allowedPriority.Contains(priority, StringComparer.OrdinalIgnoreCase)) { throw new ArgumentException("Invalid Status"); }

            Description = priority;
            return this;
        }


        /* Uso
         * 
         * Task task = Task.Create()
         * 
         * task
         *  .AssignUser(15);
         */
    }
}
