namespace GojiApi.Model.Task
{
    public class Task : ITask
    {
        public int? UserId { get; set; }

        public static Task Create()
        {
            return new Task();
        }

        public Task AssignUser(int userId)
        {
            UserId = userId;
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
