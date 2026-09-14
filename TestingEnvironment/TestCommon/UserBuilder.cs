using System;
using System.Collections.Generic;
using System.Text;
using GojiApi.Model.User;

namespace TestingEnvironment.TestCommon
{
    public class UserBuilder
    {
        private string _name = "Usuario de Prueba";
        private string? _email = "test@example.com";
        private bool _active = true;

        public UserBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserBuilder WithEmail(string? email)
        {
            _email = email;
            return this;
        }

        public UserBuilder Inactive()
        {
            _active = false;
            return this;
        }

        public User Build()
        {
            var user = User.Create(_name);

            if (_email != null)
                user.WithEmail(_email);

            if (!_active)
                user.Deactivate();

            return user;
        }
    }
}