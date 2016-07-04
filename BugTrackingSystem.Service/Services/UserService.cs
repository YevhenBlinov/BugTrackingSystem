using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
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
