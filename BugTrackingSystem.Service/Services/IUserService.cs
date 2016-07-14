using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> GetUsers(out int allUsersCount, int currentPage = 1,
            string sortBy = Constants.SortUsersByName);

        UserViewModel GetUserById(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjects(int userId);

        IEnumerable<BaseBugViewModel> GetUsersBugs(int userId, out int allBugsCount, int currentPage = 1);

        void AddUser(UserFormViewModel userFormViewModel);

        void DeleteUser(int userId);

        void EditUserInformation(EditUserFormViewModel editUserFormViewModel);

        void ChangeUserPassword(int userId, string password);

        IEnumerable<UserViewModel> SearchUsersByFirstNameAndSecondName(string fullName, out int findedUsersCount,
            int currentPage = 1, string sortBy = Constants.SortUsersByName);

        bool IsUserExists(string email, string password);

        int GetUserIdByEmail(string email);

        UserRole GetUserRoleByEmail(string email);
    }
}
