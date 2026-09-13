namespace GojiApi.Dtos
{
    public record CreateUserRequest(string Name, string Email);

    public record UserResponse(Guid Id, string Name, string Email, bool Active, DateTime? CreatedAt)
    {
        public static UserResponse From(Model.User.User user) =>
            new(user.Id, user.Name ?? string.Empty, user.Email ?? string.Empty, user.Active, user.CreatedAt);
    }
}
