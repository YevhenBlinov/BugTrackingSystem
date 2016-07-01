using System.Collections.Generic;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class BugService : IBugService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IBugRepository _bugRepository;

        public BugService(IUserRepository userRepository, IProjectRepository projectRepository, IBugRepository bugRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _bugRepository = bugRepository;
        }

        public IEnumerable<Bug> GetAllBugs()
        {
            var bugs = _bugRepository.GetAll();
            return bugs;
        }
    }
}
