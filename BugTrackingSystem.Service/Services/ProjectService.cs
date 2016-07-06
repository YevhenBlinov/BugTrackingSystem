using System;
using System.Collections.Generic;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;

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
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<ProjectViewModel> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        public ProjectViewModel GetProjectById(int projectId)
        {
            var project = _projectRepository.GetById(projectId);
            var projectModels = _mapper.Map<Project, ProjectViewModel>(project);
            return projectModels;
        }
    }
}
