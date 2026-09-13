using GojiApi.Delegate;
using GojiApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GojiApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserDelegate _userDelegate;

        public UsersController(IUserDelegate userDelegate)
        {
            _userDelegate = userDelegate;
        }

        // POST /users
        [HttpPost]
        public ActionResult<UserResponse> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = _userDelegate.RegisterUser(request.Name, request.Email);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponse.From(user));
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

        // GET /users/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<UserResponse> GetById(Guid id)
        {
            var user = _userDelegate.GetUser(id);
            return user is null ? NotFound() : UserResponse.From(user);
        }
    }
}
