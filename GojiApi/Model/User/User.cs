using System.Runtime.Serialization;

namespace GojiApi.Model.User
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool Active { get; set; }
        public DateTime? CreatedAt { get; private set; }

        public User() { }

        public static User Create(string name) => new User 
        { 
            Name = name,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Active = true
        };

        public User WithEmail(string email)
        {
            Email = email;
            return this;
        }

        public User WithPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
            return this;
        }

        public User Activate() { Active = true; return this; }

        public User Deactivate() { Active = false; return this; }
    }
}
