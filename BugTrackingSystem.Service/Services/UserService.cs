using System.Collections.Generic;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return users;
        }

        public User GetUserById(int id)
        {
            var user = _userRepository.GetById(id);
            return user;
        }
    }
}
