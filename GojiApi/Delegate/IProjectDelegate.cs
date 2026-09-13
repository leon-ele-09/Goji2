using GojiApi.Model.Project;

namespace GojiApi.Delegate
{
    public interface IProjectDelegate
    {
        
        Project CreateProject(string name, string? businessKey, Guid ownerUserId);

        Project? GetProject(Guid id);

        Project AddUserToProject(Guid projectId, Guid requestingUserId, Guid userIdToAdd);
    }
}
