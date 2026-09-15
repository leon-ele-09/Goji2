using GojiApi.Delegate;
using GojiApi.Dtos;

namespace GojiApi.Endpoints
{
    public static class ProjectsEndpoints
    {
        public static IEndpointRouteBuilder MapProjectsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/projects").WithTags("Projects");

            // POST /projects
            // Crea proyectos
            group.MapPost("/", Create);

            // GET /projects/{id}
            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetProjectById");

            // POST /projects/{id}/users
            // Mete usuarios al proyecto.
            group.MapPost("/{id:guid}/users", AddUser);

            return app;
        }

        private static IResult Create(CreateProjectRequest request, IProjectDelegate projectDelegate)
        {
            try
            {
                var project = projectDelegate.CreateProject(request.Name, request.BusinessKey, request.OwnerUserId);
                return Results.CreatedAtRoute("GetProjectById", new { id = project.Id }, ProjectResponse.From(project));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }

        private static IResult GetById(Guid id, IProjectDelegate projectDelegate)
        {
            var project = projectDelegate.GetProject(id);
            return project is null ? Results.NotFound() : Results.Ok(ProjectResponse.From(project));
        }

        private static IResult AddUser(Guid id, AddUserToProjectRequest request, IProjectDelegate projectDelegate)
        {
            try
            {
                var project = projectDelegate.AddUserToProject(id, request.RequestingUserId, request.UserIdToAdd);
                return Results.Ok(ProjectResponse.From(project));
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }
    }
}