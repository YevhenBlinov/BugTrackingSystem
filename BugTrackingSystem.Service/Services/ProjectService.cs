using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Project> GetAllProjects()
        {
            var projects = _projectRepository.GetAll();
            return projects;
        }

        public IEnumerable<Project> GetAllUsersProject()
        {
            throw new System.NotImplementedException();
        }
    }
}
