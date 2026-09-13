namespace GojiApi.Model.Project
{
    public interface IProject
    {
        void Add(Project project);
        Project? GetById(Guid id);
        Project? GetByName(string name);
        Project? GetByBusinessKey(string businessKey);
        void Update(Project project);
        void Delete(Project project);
    }
}