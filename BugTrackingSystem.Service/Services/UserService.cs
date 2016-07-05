using System.Collections.Generic;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Data.Model;
using AutoMapper;
using System;

namespace BugTrackingSystem.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>();
                cfg.CreateMap<Project, ProjectViewModel>();
                cfg.CreateMap<Bug, BugViewModel>()
                .ForMember(bgv => bgv.Status, opt => opt.MapFrom(b => Enum.GetName(typeof(BugTrackingSystem.Service.Models.BugStatus), b.StatusID)))
                .ForMember(bgv => bgv.Priority, opt => opt.MapFrom(b => Enum.GetName(typeof(BugTrackingSystem.Service.Models.BugPriority), b.PriorityID)));
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            var users = _userRepository.GetAll();
            var userModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);
            return userModels;
        }

        public UserViewModel GetUserById(int userId)
        {
            var user = _userRepository.GetById(userId);
            var userModel = _mapper.Map<User, UserViewModel>(user);
            return userModel;
        }

        public IEnumerable<ProjectViewModel> GetUsersProjects(int userId)
        {
            var user = _userRepository.GetById(userId);
            var projects = user.Projects;
            var projectModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return projectModels;
        }

        public IEnumerable<BugViewModel> GetUsersBugs(int userId)
        {
            var user = _userRepository.GetById(userId);
            var bugs = user.Bugs;
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(bugs);
            return bugModels;
        }
    }
}
