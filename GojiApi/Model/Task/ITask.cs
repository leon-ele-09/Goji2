namespace GojiApi.Model.Task
{
    public interface ITask
    {
        int? UserId { get; set; }

        static abstract Task Create();
        Task AssignUser(int userId);
    }
}