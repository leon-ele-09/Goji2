using GojiApi.Delegate;
using GojiApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GojiApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskDelegate _taskDelegate;

        public TasksController(ITaskDelegate taskDelegate)
        {
            _taskDelegate = taskDelegate;
        }

        // POST /tasks
        // Crea una tarea dentro de un proyecto y la asigna
        [HttpPost]
        public ActionResult<TaskResponse> Create([FromBody] CreateTaskRequest request)
        {
            try
            {
                var task = _taskDelegate.CreateTask(
                    request.Name, request.ProjectId, request.AssigneeId,
                    request.Description, request.Status, request.Priority);

                return CreatedAtAction(nameof(GetById), new { id = task.Id }, TaskResponse.From(task));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DelegateException ex)
            {
                return ex.ToActionResult();
            }
        }

        // GET /tasks/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<TaskResponse> GetById(Guid id)
        {
            var task = _taskDelegate.GetTask(id);
            return task is null ? NotFound() : TaskResponse.From(task);
        }

        // GET /tasks?projectId=...&assigneeId=...
        // basicamente son las tareas de Y usuarios para X proyectos
        [HttpGet]
        public ActionResult<IEnumerable<TaskResponse>> GetMany([FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId)
        {
            try
            {
                var results = _taskDelegate.GetTasks(projectId, assigneeId);
                return Ok(results.Select(TaskResponse.From));
            }
            catch (DelegateException ex)
            {
                return ex.ToActionResult();
            }
        }
    }
}
