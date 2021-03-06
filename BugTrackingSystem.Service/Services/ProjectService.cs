﻿using System;
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
        private readonly IBugRepository _bugRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, IBugRepository bugRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _bugRepository = bugRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Project, ProjectViewModel>()
                    .ForMember(pvm => pvm.UsersCount, opt => opt.MapFrom(p => p.Users.Count(u => u.DeletedOn == null)))
                    .ForMember(pvm => pvm.BugsCount,
                        opt => opt.MapFrom(p => p.Bugs.Count(b => b.AssignedUserID == null || b.User.DeletedOn == null)));
                cfg.CreateMap<ProjectFormViewModel, Project>();
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                    .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Count(p => p.IsPaused == false && p.DeletedOn == null)))
                    .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Count(b => b.Project.IsPaused == false && b.Project.DeletedOn == null)));
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<ProjectViewModel> GetProjects()
        {
            var projects = _projectRepository.GetMany(p => p.DeletedOn == null);
            var allProjectsModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return allProjectsModels;
        }

        public IEnumerable<ProjectViewModel> GetProjects(out int projectsCount, int currentPage = 1, string sortBy = Constants.SortProjectsByTitle)
        {
            var projects =_projectRepository.GetMany(p => p.DeletedOn == null);
            projectsCount = projects.Count();
            projects = SortHelper.SortProjects(projects, sortBy);
            projects = projects.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
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

            foreach (var bug in project.Bugs)
            {
                bug.StatusID = 5;
                _bugRepository.Update(bug);
            }

            _projectRepository.Update(project);
            _projectRepository.Save();
            _bugRepository.Save();
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

            var projectUsers = project.Users.Where(u => u.DeletedOn == null).ToList();
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

        public IEnumerable<ProjectViewModel> SearchProjectsByName(string searchRequest, UserRole userRole, out int findedProjectsCount, int currentPage = 1, string sortBy = Constants.SortProjectsByTitle)
        {
            if (string.IsNullOrEmpty(searchRequest))
            {
                findedProjectsCount = 0;
                return new List<ProjectViewModel>();
            }

            var findedProjects = userRole == UserRole.Administrator
                ? _projectRepository.GetMany(p => p.DeletedOn == null && p.Name.Contains(searchRequest))
                : _projectRepository.GetMany(p => p.DeletedOn == null && p.Name.Contains(searchRequest))
                    .Where(p => p.IsPaused == false);
            findedProjectsCount = findedProjects.Count();
            findedProjects = SortHelper.SortProjects(findedProjects, sortBy);
            findedProjects = findedProjects.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
            var findedProjectsViewModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(findedProjects);
            return findedProjectsViewModels;
        }

        public void AddUsersToProject(int projectId, string usersIds)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            var splittedIds = usersIds.Split(' ');

            for (int i = 0; i < splittedIds.Length - 1; i++)
            {
                var userToAdd = _userRepository.GetById(int.Parse(splittedIds[i]));
                project.Users.Add(userToAdd);
            }

            _projectRepository.Update(project);
            _projectRepository.Save();
        }
    }
}
