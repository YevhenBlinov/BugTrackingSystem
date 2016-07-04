using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BugService(IBugRepository bugRepository, IUnitOfWork unitOfWork)
        {
            _bugRepository = bugRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Bug> GetAllBugs()
        {
            var bugs = _bugRepository.GetAll();
            return bugs;
        }
    }
}
