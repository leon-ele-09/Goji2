using GojiApi.Model.User;

namespace GojiApi.Delegate
{
    public interface IUserDelegate
    {
        
        User RegisterUser(string name, string email);

        User? GetUser(Guid id);
    }
}
