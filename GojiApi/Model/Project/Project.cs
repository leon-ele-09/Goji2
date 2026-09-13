namespace GojiApi.Model.Project
{
    public class Project : IProject
    {
        public string? Name { get; set; }
        public List<int> UserIds { get; set; } = new();

        public static Project Create()
        {
            return new Project();
        }

        public Project AssignUser(int userId)
        {
            if (!UserIds.Contains(userId))
            {
                UserIds.Add(userId);
            }
            return this;
        }

        public Project RemoveUser(int userId)
        {
            UserIds.Remove(userId);
            return this;
        }

        public bool HasUser(int userId)
        {
            return UserIds.Contains(userId);
        }
    }
}
