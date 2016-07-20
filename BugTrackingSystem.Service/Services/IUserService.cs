using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> GetUsers();

        IEnumerable<UserViewModel> GetUsers(out int allUsersCount, int currentPage = 1,
            string sortBy = Constants.SortUsersByName);

        UserViewModel GetUserById(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjects(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjectsByPage(int userId, out int projectsCount, int currentPage = 1,
            string sortBy = Constants.SortUsersByName);
        
        IEnumerable<BaseBugViewModel> GetUsersBugs(int userId, out int allBugsCount, int currentPage = 1);

        void AddUser(UserFormViewModel userFormViewModel);

        void DeleteUser(int userId);

        void EditUserInformation(EditUserFormViewModel editUserFormViewModel);

        void ChangeUserPassword(int userId, string password);

        IEnumerable<UserViewModel> SearchUsersByFirstNameAndSecondName(string fullName, out int findedUsersCount,
            int currentPage = 1, string sortBy = Constants.SortUsersByName);

        bool IsUserExists(string email, string password);

        UserViewModel GetUserByEmail(string email);

        IEnumerable<UserViewModel> GetNotAssignedToProjectUsers(int projectId);

        void SendResetPasswordEmailToUser(string email);

        bool IsEmailValid(string email);

        int GetUserIdByCryptEmail(string hashEmail);
    }
}
