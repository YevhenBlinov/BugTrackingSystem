using System;
using System.Collections.Generic;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IEnumerable<Project> GetAllProjects()
        {
            var projects = _projectRepository.GetAll();
            return projects;
        }

        public IEnumerable<Project> GetAllUsersProject()
        {
            throw new NotImplementedException();
        }
    }
}
