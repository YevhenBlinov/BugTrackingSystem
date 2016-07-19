using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly BlobService _blobService;

        public UserService(IUserRepository userRepository, IProjectRepository projectRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                    .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Count(p => p.IsPaused == false && p.DeletedOn == null)))
                    .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Count(b => b.Project.IsPaused == false && b.Project.DeletedOn == null)));
                cfg.CreateMap<Project, ProjectViewModel>()
                    .ForMember(pvm => pvm.UsersCount, opt => opt.MapFrom(p => p.Users.Count(u => u.DeletedOn == null)))
                    .ForMember(pvm => pvm.BugsCount,
                        opt => opt.MapFrom(p => p.Bugs.Count(b => b.AssignedUserID == null || b.User.DeletedOn == null)));
                cfg.CreateMap<Bug, BaseBugViewModel>()
                    .ForMember(bgv => bgv.Status, opt => opt.MapFrom(b => (BugStatus) b.StatusID))
                    .ForMember(bgv => bgv.Priority, opt => opt.MapFrom(b => (BugPriority) b.PriorityID));
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
                        opt => opt.MapFrom(ufvm => (byte) ((UserRole) Enum.Parse(typeof (UserRole), ufvm.Role))))
                    .ForMember(u => u.FirstName, opt => opt.MapFrom(ufvm => ufvm.FirstName))
                    .ForMember(u => u.LastName, opt => opt.MapFrom(ufvm => ufvm.LastName))
                    .ForMember(u => u.Password, opt => opt.MapFrom(ufvm => ufvm.Password));
            });

            _mapper = config.CreateMapper();
            _blobService = new BlobService(Constants.UsersPhotosContainerName);
        }

        public IEnumerable<UserViewModel> GetUsers()
        {
            var users =
                _userRepository.GetMany(u => u.DeletedOn == null).ToList();
            var userModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users).ToList();

            for (var i = 0; i < userModels.Count; i++)
            {
                userModels[i].Photo = _blobService.GetBlobSasUri(users[i].Photo);
            }

            return userModels;
        }

        public IEnumerable<UserViewModel> GetUsers(out int allUsersCount, int currentPage = 1, string sortBy = Constants.SortUsersByName)
        {
            var users =
                _userRepository.GetMany(u => u.DeletedOn == null).ToList();
            allUsersCount = users.Count;
            users = SortHelper.SortUsers(users, sortBy);
            users = users.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize).ToList();
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

            if (user.DeletedOn != null)
                throw new Exception("Sorry, but the user was deleted.");

            var userModel = _mapper.Map<User, UserViewModel>(user);
            userModel.Photo = _blobService.GetBlobSasUri(user.Photo);
            return userModel;
        }

        public IEnumerable<ProjectViewModel> GetUsersProjects(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            var projects = user.Projects.Where(p => p.IsPaused == false && p.DeletedOn == null);
            var projectModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return projectModels;
        }

        public IEnumerable<ProjectViewModel> GetUsersProjectsByPage(int userId, out int projectsCount, int currentPage = 1,
            string sortBy = Constants.SortUsersByName)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            var projects = user.Projects.Where(p => p.IsPaused == false && p.DeletedOn == null);
            projectsCount = projects.Count();
            projects = SortHelper.SortProjects(projects, sortBy);
            projects = projects.Skip((currentPage - 1)*Constants.StickerPageSize).Take(Constants.StickerPageSize);
            var projectModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectViewModel>>(projects);
            return projectModels;
        }

        public IEnumerable<BaseBugViewModel> GetUsersBugs(int userId, out int allBugsCount, int currentPage = 1)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            var bugs = user.Bugs.Where(b => b.Project.IsPaused == false && b.Project.DeletedOn == null);
            allBugsCount = bugs.Count();
            bugs = bugs.Skip((currentPage - 1)*Constants.ListPageSize).Take(Constants.ListPageSize);
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BaseBugViewModel>>(bugs);
            return bugModels;
        }

        public void AddUser(UserFormViewModel userFormViewModel)
        {
            var allUsers = _userRepository.GetAll();
            var isUserExist =
                allUsers.Any(
                    u =>
                        u.FirstName == userFormViewModel.FirstName && u.LastName == userFormViewModel.LastName &&
                        u.Email == userFormViewModel.Email);

            if (isUserExist)
                throw new Exception("Sorry, but the user with the same name, surname and email already exists.");

            var userToAdd = _mapper.Map<UserFormViewModel, User>(userFormViewModel);
            userToAdd.Password = CalculateMd5Hash(userToAdd.Password);
            userToAdd.Photo = Constants.DefaultUserIconName;
            userToAdd.DeletedOn = null;

            _userRepository.Add(userToAdd);
            _userRepository.Save();
        }

        public void DeleteUser(int userId)
        {
            if (userId == 1)
                throw new Exception("Sorry, but you can't delete the user, because he's super administrator.");

            var userToDelete = _userRepository.GetById(userId);

            if (userToDelete == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            userToDelete.DeletedOn = DateTime.Now;

            if (userToDelete.Photo != Constants.DefaultUserIconName)
            {
                _blobService.DeleteBlobFromContainer(userToDelete.Photo);
                userToDelete.Photo = Constants.DefaultUserIconName;
            }

            _userRepository.Update(userToDelete);
            _userRepository.Save();
        }

        public void EditUserInformation(EditUserFormViewModel editUserFormViewModel)
        {
            var userToEdit = _userRepository.GetById(editUserFormViewModel.UserId);
            userToEdit.FirstName = editUserFormViewModel.FirstName;
            userToEdit.LastName = editUserFormViewModel.LastName;
            userToEdit.Email = editUserFormViewModel.Email;
            userToEdit.Login = editUserFormViewModel.Email;
            userToEdit.UserRoleID = (byte) ((UserRole) Enum.Parse(typeof (UserRole), editUserFormViewModel.Role));

            if (editUserFormViewModel.IsPhotoEdited)
            {
                if (editUserFormViewModel.Photo != null)
                {
                    _blobService.UploadBlobIntoContainerFromByteArray(userToEdit.FirstName + userToEdit.LastName,
                        editUserFormViewModel.Photo);
                    userToEdit.Photo = userToEdit.FirstName + userToEdit.LastName;
                }
                else
                {
                    userToEdit.Photo = Constants.DefaultUserIconName;
                }
            }

            _userRepository.Update(userToEdit);
            _userRepository.Save();
        }

        public void ChangeUserPassword(int userId, string password)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new Exception("Sorry, but the user doesn't exist.");

            user.Password = CalculateMd5Hash(password);
            _userRepository.Update(user);
            _userRepository.Save();
        }

        public IEnumerable<UserViewModel> SearchUsersByFirstNameAndSecondName(string fullName, out int findedUsersCount, int currentPage = 1, string sortBy = Constants.SortUsersByName)
        {
            var splitFullName = fullName.Split(' ');

            switch (splitFullName.Length)
            {
                case 1:
                {
                    var name = splitFullName[0];
                    var findedUsers =
                        _userRepository.GetMany(
                            u => u.DeletedOn == null && (u.FirstName.Contains(name) || u.LastName.Contains(name)))
                            .ToList();

                    if (!findedUsers.Any())
                    {
                        findedUsersCount = 0;
                        return new List<UserViewModel>();
                    }

                    findedUsersCount = findedUsers.Count;
                    findedUsers = SortHelper.SortUsers(findedUsers, sortBy);
                    findedUsers = findedUsers.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize).ToList();

                    var findedUsersViewModels =
                        _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(findedUsers).ToList();

                    for (var i = 0; i < findedUsersViewModels.Count; i++)
                    {
                        findedUsersViewModels[i].Photo = _blobService.GetBlobSasUri(findedUsers[i].Photo);
                    }

                    return findedUsersViewModels;
                }
                case 2:
                {
                    var firstName = splitFullName[0];
                    var lastName = splitFullName[1];

                    var findedUsers =
                        _userRepository.GetMany(
                            u => u.DeletedOn == null && u.FirstName.Contains(firstName) && u.LastName.Contains(lastName))
                            .ToList();

                    if (!findedUsers.Any())
                    {
                        findedUsersCount = 0;
                        return new List<UserViewModel>();
                    }

                    findedUsersCount = findedUsers.Count;
                    findedUsers =
                        findedUsers.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.ListPageSize).ToList();
                    var findedUsersViewModels =
                        _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(findedUsers).ToList();

                    for (var i = 0; i < findedUsersViewModels.Count; i++)
                    {
                        findedUsersViewModels[i].Photo = _blobService.GetBlobSasUri(findedUsers[i].Photo);
                    }

                    return findedUsersViewModels;
                }
                default:
                {
                    findedUsersCount = 0;
                    return new List<UserViewModel>();
                }
            }
        }

        public bool IsUserExists(string email, string password)
        {
            var hashPassword = CalculateMd5Hash(password);
            var user = _userRepository.Get(u => u.DeletedOn == null && u.Email == email && u.Password == hashPassword);
            return user != null;
        }

        private string CalculateMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            foreach (var c in hash)
            {
                sb.Append(c.ToString("X2"));
            }

            return sb.ToString();
        }

        public UserViewModel GetUserByEmail(string email)
        {
            var user = _userRepository.Get(u => u. Email == email);

            if (user.DeletedOn != null)
                throw new Exception("Sorry, but the user was deleted.");

            var userModel = _mapper.Map<User, UserViewModel>(user);
            userModel.Photo = _blobService.GetBlobSasUri(user.Photo);
            return userModel;
        }

        public IEnumerable<UserViewModel> GetNotAssignedToProjectUsers(int projectId)
        {
            var project = _projectRepository.GetById(projectId);

            if (project == null)
                throw new Exception("Sorry, but the project doesn't exist.");

            var projectUsers = _userRepository.GetMany(u => u.DeletedOn == null).Where(u => u.Projects.Contains(project));
            var notAssignedUsers = _userRepository.GetMany(u => u.DeletedOn == null).Except(projectUsers).ToList();
            var notAssignedUsersViewModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(notAssignedUsers).ToList();

            for (var i = 0; i < notAssignedUsersViewModels.Count; i++)
            {
                notAssignedUsersViewModels[i].Photo = _blobService.GetBlobSasUri(notAssignedUsers[i].Photo);
            }

            return notAssignedUsersViewModels;
        }

        public void SendResetPasswordEmailToUser(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user.DeletedOn != null)
                throw new Exception("Sorry, but the user was deleted.");

            BusQueueService.AddResetPasswordMessageToQueue(user.FirstName, user.Email,
                "http://asignar.azurewebsites.net/Login/ResetPassword/?" + userId);
        }

        public bool IsEmailValid(string email)
        {
            var user = _userRepository.Get(u => u.DeletedOn == null && u.Email == email);
            return user != null;
        }
    }
}
