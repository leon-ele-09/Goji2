using GojiApi.Delegate;
using GojiApi.Dtos;

namespace GojiApi.Endpoints
{
    public static class UsersEndpoints
    {
        public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/users").WithTags("Users");

            // POST /users
            group.MapPost("/", Create);

            // GET /users/{id}
            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetUserById");

            return app;
        }

        private static IResult Create(CreateUserRequest request, IUserDelegate userDelegate)
        {
            try
            {
                var user = userDelegate.RegisterUser(request.Name, request.Email);
                return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, UserResponse.From(user));
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

        private static IResult GetById(Guid id, IUserDelegate userDelegate)
        {
            var user = userDelegate.GetUser(id);
            return user is null ? Results.NotFound() : Results.Ok(UserResponse.From(user));
        }
    }
}