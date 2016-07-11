using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<UserViewModel> GetAllUsers();

        UserViewModel GetUserById(int userId);

        IEnumerable<ProjectViewModel> GetUsersProjects(int userId);

        IEnumerable<BaseBugViewModel> GetUsersBugs(int userId);

        void AddUser(string firstName, string lastName, string email, string password, string role);
    }
}
