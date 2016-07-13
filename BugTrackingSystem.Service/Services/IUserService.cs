using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> GetUsers(int currentPage = 1, string sortBy = Constants.SortUsersByName);

        int GetUsersCount();

        UserViewModel GetUserById(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjects(int userId);

        IEnumerable<BaseBugViewModel> GetUsersBugs(int userId, int currentPage = 1);

        int GetUsersBugsCount(int userId);

        void AddUser(UserFormViewModel userFormViewModel);

        void DeleteUser(int userId);

        void EditUserInformation(EditUserFormViewModel editUserFormViewModel);

        void ChangeUserPassword(int userId, string password);

        IEnumerable<UserViewModel> SearchUsersByFirstNameAndSecondName(string fullName, int currentPage = 1);

        int GetFindedUsersByFirstNameAndSecondNameCount(string fullName);
    }
}
