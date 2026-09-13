namespace GojiApi.Model.Task
{
    public interface ITask
    {
        int? UserId { get; set; }
        string? Name { get; set; }

        static abstract Task Create();
        Task AssignUser(int userId);
        Task AssignName(string name);
    }
}