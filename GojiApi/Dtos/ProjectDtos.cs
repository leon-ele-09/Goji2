namespace GojiApi.Dtos
{
    public record CreateProjectRequest(string Name, string? BusinessKey, Guid OwnerUserId);

    public record AddUserToProjectRequest(Guid RequestingUserId, Guid UserIdToAdd);

    // queda redundante pq usa lo mismo que el proyecto
    public record ProjectResponse(Guid Id, string Name, string? BusinessKey, List<Guid> UserIds, DateTime? CreatedAt)
    {
        public static ProjectResponse From(Model.Project.Project project) =>
            new(project.Id, project.Name ?? string.Empty, project.BusinessKey, project.UserIds, project.CreatedAt);
    }
}
