using System.Collections.Generic;

namespace BugTrackingSystem.Service.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);
    }
}
