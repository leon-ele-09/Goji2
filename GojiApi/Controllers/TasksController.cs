using GojiApi.Delegate;
using GojiApi.Dtos;

namespace GojiApi.Endpoints
{
    public static class TasksEndpoints
    {
        public static IEndpointRouteBuilder MapTasksEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/tasks").WithTags("Tasks");

            // POST /tasks
            // Crea una tarea dentro de un proyecto y la asigna
            group.MapPost("/", Create);

            // GET /tasks/{id}
            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetTaskById");

            // GET /tasks?projectId=...&assigneeId=...
            // basicamente son las tareas de Y usuarios para X proyectos
            group.MapGet("/", GetMany);

            return app;
        }

        private static IResult Create(CreateTaskRequest request, ITaskDelegate taskDelegate)
        {
            try
            {
                var task = taskDelegate.CreateTask(
                    request.Name, request.ProjectId, request.AssigneeId,
                    request.Description, request.Status, request.Priority);

                return Results.CreatedAtRoute("GetTaskById", new { id = task.Id }, TaskResponse.From(task));
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

        private static IResult GetById(Guid id, ITaskDelegate taskDelegate)
        {
            var task = taskDelegate.GetTask(id);
            return task is null ? Results.NotFound() : Results.Ok(TaskResponse.From(task));
        }

        private static IResult GetMany(Guid? projectId, Guid? assigneeId, ITaskDelegate taskDelegate)
        {
            try
            {
                var results = taskDelegate.GetTasks(projectId, assigneeId);
                return Results.Ok(results.Select(TaskResponse.From));
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }
    }
}