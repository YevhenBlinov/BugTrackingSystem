using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FilterService(IFilterRepository filterRepository, IUnitOfWork unitOfWork)
        {
            _filterRepository = filterRepository;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Filter> GetAllFilters()
        {
            var filter = _filterRepository.GetAll();
            return filter;
        }
    }
}
