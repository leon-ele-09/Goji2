namespace GojiApi.Model.Project
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string? BusinessKey { get; set; }
        public string? Name { get; set; }
        public List<Guid> UserIds { get; set; } = new();
        public DateTime? CreatedAt { get; private set; }

        public static Project Create() => new Project
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        // esto es para como reconstruir otras cosas
        internal static Project Hydrate(Guid id, string name, string? businessKey, List<Guid> userIds, DateTime createdAt) =>
            new Project
            {
                Id = id,
                Name = name,
                BusinessKey = businessKey,
                UserIds = userIds,
                CreatedAt = createdAt
            };

        public Project WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("Invalid Name"); }

            Name = name;
            return this;
        }

        public Project WithBusinessKey(string businessKey)
        {
            if (string.IsNullOrWhiteSpace(businessKey)) { throw new ArgumentException("Invalid BusinessKey"); }

            BusinessKey = businessKey;
            return this;
        }

        public bool HasUser(Guid Id)
        {
            return UserIds.Contains(Id);
        }

        public Project AddUser(Guid Id)
        {
            if (!UserIds.Contains(Id))
            {
                UserIds.Add(Id);
            }
            return this;
        }

        public Project RemoveUser(Guid Id)
        {
            if (UserIds.Contains(Id))
            {
                UserIds.Remove(Id);
            }
            return this;
        }

        

    }
}
