using System;
using System.Collections.Generic;
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
            var allProjects = _projectRepository.GetAll();
            var allProjectsModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(allProjects);
            return allProjectsModels;
        }

        public ProjectViewModel GetProjectById(int projectId)
        {
            var project = _projectRepository.GetById(projectId);
            var projectModels = _mapper.Map<Project, ProjectViewModel>(project);
            return projectModels;
        }

        public void AddNewProject(string name, string prefix)
        {
            var projectViewModel = new ProjectFormViewModel(){Name = name, Prefix = prefix};
            var project = _mapper.Map<ProjectFormViewModel, Project>(projectViewModel);
            _projectRepository.Add(project);
            _projectRepository.Save();
        }

        public void DeleteProject(int projectId)
        {
            _projectRepository.Delete(p => p.ProjectID == projectId);
            _projectRepository.Save();
        }
    }
}
