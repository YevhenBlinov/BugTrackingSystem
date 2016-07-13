using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> GetAllUsers(string sortBy = Constants.SortUsersByName);

        UserViewModel GetUserById(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjects(int userId);

        IEnumerable<BaseBugViewModel> GetUsersBugs(int userId);

        void AddUser(UserFormViewModel userFormViewModel);

        void DeleteUser(int userId);

        void EditUserInformation(EditUserFormViewModel editUserFormViewModel);

        void ChangeUserPassword(int userId, string password);

        IEnumerable<UserViewModel> SearchUserByFirstNameAndSecondName(string fullName);
    }
}
