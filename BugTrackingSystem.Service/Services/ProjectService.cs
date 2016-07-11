using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

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
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<ProjectViewModel> GetAllProjects()
        {
            var allProjects = _projectRepository.GetMany(p => p.DeletedOn == null);
            var allProjectsModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(allProjects);
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
    }
}
