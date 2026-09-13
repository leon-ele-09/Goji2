using GojiApi.Model.User;

namespace GojiApi.Delegate
{
    public class UserDelegate : IUserDelegate
    {
        private readonly IUser _users;

        public UserDelegate(IUser users)
        {
            _users = users;
        }

        public User RegisterUser(string name, string email)
        {
         
            var user = User.Create(name).WithEmail(email);
            
            if(_users.GetByName(user.Name!) is not null)
            {
                throw new ConflictException($"Alguien mas tiene este username : '{user.Name}'");
            }

         
            if (_users.GetByEmail(user.Email!) is not null)
            {
                throw new ConflictException($"Alguien mas tiene este mail : '{user.Email}'");
            }

            _users.Add(user);
            return user;
        }

        public User? GetUser(Guid id) => _users.GetById(id);
    }
}
