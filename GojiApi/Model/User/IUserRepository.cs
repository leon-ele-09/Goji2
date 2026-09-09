namespace GojiApi.Model.User
{
    public interface IUserRepository
    {
        void Add(User user);
        User GetById(Guid id);
        User GetByName(string name);
        User GetByEmail(string email);
        void Update(User user);
        void Delete(User user);
    }
}
