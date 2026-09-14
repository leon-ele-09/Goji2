using System;
using System.Collections.Generic;
using System.Text;
using GojiApi.Model.Project;

namespace TestingEnvironment.TestCommon
{
    public class ProjectBuilder
    {
        private string _name = "Proyecto de Prueba";
        private string? _businessKey = "TEST";
        private readonly List<Guid> _userIds = new();

        public ProjectBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProjectBuilder WithBusinessKey(string? businessKey)
        {
            _businessKey = businessKey;
            return this;
        }

        public ProjectBuilder WithUsers(params Guid[] userIds)
        {
            _userIds.AddRange(userIds);
            return this;
        }

        public Project Build()
        {
            var project = Project.Create().WithName(_name);

            if (_businessKey != null)
                project.WithBusinessKey(_businessKey);

            foreach (var id in _userIds)
                project.AddUser(id);

            return project;
        }

        public static Project Empty() => new ProjectBuilder().Build();
    }
}