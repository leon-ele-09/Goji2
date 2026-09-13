namespace GojiApi.Model.Task
{
    public class Task : ITask
    {
        public int? UserId { get; set; }
        public string? Name { get; set; }

        public static Task Create()
        {
            return new Task();
        }

        public Task AssignUser(int userId)
        {
            UserId = userId;
            return this;
        }

        public Task AssignName(string name)
        {
            Name = name; 
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
