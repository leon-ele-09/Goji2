using GojiApi.Model.Project;
using GojiApi.Model.User;

namespace GojiApi.Delegate
{
    // el delegate tiene ya las reglas bien
    public class ProjectDelegate : IProjectDelegate
    {
        private readonly IProject _projects;
        private readonly IUser _users;

        public ProjectDelegate(IProject projects, IUser users)
        {
            _projects = projects;
            _users = users;
        }

        public Project CreateProject(string name, string? businessKey, Guid ownerUserId)
        {
            var owner = _users.GetById(ownerUserId)
                ?? throw new NotFoundException($"Userio '{ownerUserId}' no existe.");

            // Checa la clave para el proyecto
            var project = Project.Create().WithName(name);
            if (!string.IsNullOrWhiteSpace(businessKey))
            {
                project = project.WithBusinessKey(businessKey);
            }

            // Se agrega al creador. Despues le metemos roles.
            project.AddUser(owner.Id);

            _projects.Add(project);
            return project;
        }

        public Project? GetProject(Guid id) => _projects.GetById(id);

        public Project AddUserToProject(Guid projectId, Guid requestingUserId, Guid userIdToAdd)
        {
            var project = _projects.GetById(projectId)
                ?? throw new NotFoundException($"Proyecto '{projectId}' no existe.");

            // Solo alguien que este dentro puede meter
            if (!project.HasUser(requestingUserId))
            {
                throw new ForbiddenException("Solo puedes meter si estas adentro.");
            }

            var userToAdd = _users.GetById(userIdToAdd)
                ?? throw new NotFoundException($"Usuario '{userIdToAdd}' no existe.");

            project.AddUser(userToAdd.Id);
            _projects.Update(project);
            return project;
        }
    }
}
