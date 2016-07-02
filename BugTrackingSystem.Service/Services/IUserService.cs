using System.Collections.Generic;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);
    }
}
