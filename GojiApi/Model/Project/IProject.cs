namespace GojiApi.Model.Project
{
    public interface IProject
    {
        global::System.String Name { get; set; }
        List<global::System.Int32> UserIds { get; set; }

        static abstract Project Create();
        Project AssignUser(global::System.Int32 userId);
        global::System.Boolean HasUser(global::System.Int32 userId);
        Project RemoveUser(global::System.Int32 userId);
    }
}