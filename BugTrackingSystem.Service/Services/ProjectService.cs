using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BugTrackingSystem.AzureService;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using UserRole = BugTrackingSystem.Service.Models.UserRole;

namespace BugTrackingSystem.Service.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Project, ProjectViewModel>()
                .ForMember(pvm => pvm.BugsCount, opt => opt.MapFrom(p => p.Bugs.Count))
                .ForMember(pvm => pvm.UsersCount, opt => opt.MapFrom(p => p.Users.Count));
                cfg.CreateMap<ProjectFormViewModel, Project>();
                cfg.CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Where(p => p.DeletedOn == null).Count(p => p.IsPaused == false)))
                .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Where(b => b.Project.DeletedOn == null).Count(b => b.Project.IsPaused == false)));
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<ProjectViewModel> GetProjects(out int projectsCount, int currentPage = 1, string sortBy = Constants.SortProjectsByTitle)
        {
            var projects =_projectRepository.GetMany(p => p.DeletedOn == null);
            projectsCount = projects.Count();
            projects = SortHelper.SortProjects(projects, sortBy);
            projects = projects.Skip((currentPage - 1)*Constants.PageSize).Take(Constants.PageSize);
            var allProjectsModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return allProjectsModels;
        }

        public ProjectViewModel GetProjectById(int projectId)
        {
            var project = _projectRepository.GetById(projectId);

            if(project.DeletedOn != null)
                throw new Exception("Sorry, but the project was deleted.");

            var projectModel = _mapper.Map<Project, ProjectViewModel>(project);
            return projectModel;
        }

        public void AddNewProject(string name, string prefix)
        {
            var allProjects = _projectRepository.GetAll();
            var isProjectWithTheNameAndThePrefixExists = allProjects.Any(p => p.Name == name && p.Prefix == prefix);

            if(isProjectWithTheNameAndThePrefixExists)
                throw new Exception("Sorry, you can't add the project, because a project with same name and same prefix already exists.");

            var projectViewModel = new ProjectFormViewModel(){Name = name, Prefix = prefix};
            var project = _mapper.Map<ProjectFormViewModel, Project>(projectViewModel);
            _projectRepository.Add(project);
            _projectRepository.Save();
        }

        public void UpdateProjectName(int projectId, string name)
        {
            var project = _projectRepository.GetById(projectId);

            if(project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            project.Name = name;
            _projectRepository.Update(project);
            _projectRepository.Save();
        }

        public IEnumerable<ProjectViewModel> GetAllRunningProjects()
        {
            var allRunningProjects = _projectRepository.GetMany(p => p.DeletedOn == null && p.IsPaused == false);
            var allRunningProjectsModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(allRunningProjects);
            return allRunningProjectsModels;
        }

        public void DeleteProject(int projectId)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            project.DeletedOn = DateTime.Now;
            _projectRepository.Update(project);
            _projectRepository.Save();
        }

        public void PauseAndUnpauseProject(int projectId)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            project.IsPaused = !project.IsPaused;
            _projectRepository.Update(project);
            _projectRepository.Save();
        }

        public void RemoveUserFromProject(int projectId, int userId)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            var userToRemove = project.Users.First(u => u.UserID == userId);
            project.Users.Remove(userToRemove);
            _projectRepository.Update(project);
            _projectRepository.Save();
        }

        public IEnumerable<UserViewModel> GetAllProjectUsers(int projectId)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            var projectUsers = project.Users.ToList();
            var projectUsersViewModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(projectUsers).ToList();

            if (projectUsersViewModels.Count == 0) 
                return projectUsersViewModels;

            var blobService = new BlobService(Constants.UsersPhotosContainerName);

            for (var i = 0; i < projectUsersViewModels.Count; i++)
            {
                projectUsersViewModels[i].Photo = blobService.GetBlobSasUri(projectUsers[i].Photo);
            }

            return projectUsersViewModels;
        }

        public IEnumerable<ProjectViewModel> SearchProjectsByName(string searchRequest, out int findedProjectsCount, int currentPage = 1, string sortBy = Constants.SortProjectsByTitle)
        {
            if (string.IsNullOrEmpty(searchRequest))
            {
                findedProjectsCount = 0;
                return new List<ProjectViewModel>();
            }

            var findedProjects =_projectRepository.GetMany(p => p.DeletedOn == null && p.Name.Contains(searchRequest));
            findedProjectsCount = findedProjects.Count();
            findedProjects = SortHelper.SortProjects(findedProjects, sortBy);
            findedProjects = findedProjects.Skip((currentPage - 1)*Constants.PageSize).Take(Constants.PageSize);
            var findedProjectsViewModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(findedProjects);
            return findedProjectsViewModels;
        }
    }
}
