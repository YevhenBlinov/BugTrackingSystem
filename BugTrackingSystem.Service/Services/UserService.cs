using System;
using System.Collections.Generic;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugPriority = BugTrackingSystem.Service.Models.BugPriority;
using BugStatus = BugTrackingSystem.Service.Models.BugStatus;

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
                cfg.CreateMap<Bug, BaseBugViewModel>()
                .ForMember(bgv => bgv.Status, opt => opt.MapFrom(b => Enum.GetName(typeof(BugStatus), b.StatusID)))
                .ForMember(bgv => bgv.Priority, opt => opt.MapFrom(b => Enum.GetName(typeof(BugPriority), b.PriorityID)));
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

        public IEnumerable<BaseBugViewModel> GetUsersBugs(int userId)
        {
            var user = _userRepository.GetById(userId);
            var bugs = user.Bugs;
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BaseBugViewModel>>(bugs);
            return bugModels;
        }
    }
}
