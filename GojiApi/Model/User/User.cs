using System.Runtime.Serialization;

namespace GojiApi.Model.User
{
    public class User
    {
        public Guid Id { get; private set; }
        public string? Name { get; set; }
        public string? Email { get; set; } = null!;
        // public string PasswordHash { get; set; } = null!; ya que le metamos autenticacion
        public bool Active { get; set; }
        public DateTime? CreatedAt { get; private set; }

        public static User Create(string name) => new User 
        { 
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Active = true
        }.WithName(name);

        internal static User Hydrate(Guid id, string name, string email, bool active, DateTime createdAt) =>
            new User
            {
                Id = id,
                Name = name,
                Email = email,
                Active = active,
                CreatedAt = createdAt
            };

        public User WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("Invalid Name"); }

            Name = name;
            return this;
        }

        public User WithEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) { throw new ArgumentException("Invalid Email"); }  

            Email = email;
            return this;
        }

        /*
        public User WithPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
            return this;
        }
        */

        public User Activate() { Active = true; return this; }

        public User Deactivate() { Active = false; return this; }
    }
}
