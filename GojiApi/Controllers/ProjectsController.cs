using GojiApi.Delegate;
using GojiApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GojiApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectDelegate _projectDelegate;

        public ProjectsController(IProjectDelegate projectDelegate)
        {
            _projectDelegate = projectDelegate;
        }

        // POST /projects
        // Crea proyectos
        [HttpPost]
        public ActionResult<ProjectResponse> Create([FromBody] CreateProjectRequest request)
        {
            try
            {
                var project = _projectDelegate.CreateProject(request.Name, request.BusinessKey, request.OwnerUserId);
                return CreatedAtAction(nameof(GetById), new { id = project.Id }, ProjectResponse.From(project));
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

        // GET /projects/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<ProjectResponse> GetById(Guid id)
        {
            var project = _projectDelegate.GetProject(id);
            return project is null ? NotFound() : ProjectResponse.From(project);
        }

        // POST /projects/{id}/users
        // Mete usuarios al proyecto.
        [HttpPost("{id:guid}/users")]
        public ActionResult<ProjectResponse> AddUser(Guid id, [FromBody] AddUserToProjectRequest request)
        {
            try
            {
                var project = _projectDelegate.AddUserToProject(id, request.RequestingUserId, request.UserIdToAdd);
                return ProjectResponse.From(project);
            }
            catch (DelegateException ex)
            {
                return ex.ToActionResult();
            }
        }
    }
}
