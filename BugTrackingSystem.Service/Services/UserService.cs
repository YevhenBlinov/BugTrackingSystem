using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BugTrackingSystem.AzureService;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using BugPriority = BugTrackingSystem.Service.Models.BugPriority;
using BugStatus = BugTrackingSystem.Service.Models.BugStatus;
using UserRole = BugTrackingSystem.Service.Models.UserRole;

namespace BugTrackingSystem.Service.Services
{
    public class UserService : IUserService
    {
        public const string UsersPhotosContainerName = "usersphotos";
        private const string DefaultUserIconName = "DefaultUserIcon.png";
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly BlobService _blobService;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Count))
                .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Count));
                cfg.CreateMap<Project, ProjectViewModel>();
                cfg.CreateMap<Bug, BaseBugViewModel>()
                .ForMember(bgv => bgv.Status, opt => opt.MapFrom(b => (BugStatus)b.StatusID))
                .ForMember(bgv => bgv.Priority, opt => opt.MapFrom(b => (BugPriority)b.PriorityID));
                cfg.CreateMap<UserFormViewModel, User>()
                    .ForMember(u => u.Projects, opt => opt.Ignore())
                    .ForMember(u => u.Bugs, opt => opt.Ignore())
                    .ForMember(u => u.UserID, opt => opt.Ignore())
                    .ForMember(u => u.DeletedOn, opt => opt.Ignore())
                    .ForMember(u => u.Filters, opt => opt.Ignore())
                    .ForMember(u => u.Photo, opt => opt.Ignore())
                    .ForMember(u => u.Login, opt => opt.MapFrom(ufvm => ufvm.Email))
                    .ForMember(u => u.Email, opt => opt.MapFrom(ufvm => ufvm.Email))
                    .ForMember(u => u.UserRoleID,
                        opt => opt.MapFrom(ufvm => (byte) ((UserRole)Enum.Parse(typeof (UserRole), ufvm.Role))))
                    .ForMember(u => u.FirstName, opt => opt.MapFrom(ufvm => ufvm.FirstName))
                    .ForMember(u => u.LastName, opt => opt.MapFrom(ufvm => ufvm.LastName))
                    .ForMember(u => u.Password, opt => opt.MapFrom(ufvm => ufvm.Password));
            });

            _mapper = config.CreateMapper();
            _blobService = new BlobService(UsersPhotosContainerName);
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            var users = _userRepository.GetMany(u => u.DeletedOn == null).ToList();
            var userModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users).ToList();

            for (var i = 0; i < userModels.Count; i++)
            {
                userModels[i].Photo = _blobService.GetBlobSasUri(users[i].Photo);
            }

            return userModels;
        }

        public UserViewModel GetUserById(int userId)
        {
            var user = _userRepository.GetById(userId);

            if(user.DeletedOn != null)
                throw new Exception("Sorry, but the user was deleted.");

            var userModel = _mapper.Map<User, UserViewModel>(user);
            userModel.Photo = _blobService.GetBlobSasUri(user.Photo);
            return userModel;
        }

        public IEnumerable<ProjectViewModel> GetUsersProjects(int userId)
        {
            var user = _userRepository.GetById(userId);

            if(user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            var projects = user.Projects.Where(p => p.DeletedOn == null);
            var projectModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return projectModels;
        }

        public IEnumerable<BaseBugViewModel> GetUsersBugs(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            var bugs = user.Bugs.Where(b => b.Project.DeletedOn == null);
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BaseBugViewModel>>(bugs);
            return bugModels;
        }

        public void AddUser(UserFormViewModel userFormViewModel)
        {
            var allUsers = _userRepository.GetAll();
            var isUserExist = allUsers.Any(u => u.FirstName == userFormViewModel.FirstName && u.LastName == userFormViewModel.LastName && u.Email == userFormViewModel.Email);

            if(isUserExist)
                throw new Exception("Sorry, but the user with the same name, surname and email already exists.");

            var userToAdd = _mapper.Map<UserFormViewModel, User>(userFormViewModel);
            userToAdd.Photo = DefaultUserIconName;
            userToAdd.DeletedOn = null;

            _userRepository.Add(userToAdd);
            _userRepository.Save();
        }

        public void DeleteUser(int userId)
        {
            if (userId == 1)
                throw new Exception("Sorry, but you can't delete the user, because he's super administrator.");

            var userToDelete = _userRepository.GetById(userId);

            if(userToDelete == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            userToDelete.DeletedOn = DateTime.Now;

            if (userToDelete.Photo != DefaultUserIconName)
            {
                _blobService.DeleteBlobFromContainer(userToDelete.Photo);
                userToDelete.Photo = DefaultUserIconName;
            }

            _userRepository.Update(userToDelete);
            _userRepository.Save();
        }
    }
}
