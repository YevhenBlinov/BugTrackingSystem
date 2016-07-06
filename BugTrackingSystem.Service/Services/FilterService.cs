using System.Collections.Generic;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IMapper _mapper;

        public FilterService(IFilterRepository filterRepository)
        {
            _filterRepository = filterRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Filter, FilterViewModel>();
            });

            _mapper = config.CreateMapper();
        }
        public IEnumerable<FilterViewModel> GetAllUserFilters(int userId)
        {
            var filters = _filterRepository.GetMany(f => f.UserID == userId);
            var filterModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(filters);
            return filterModels;
        }

        public void DeleteFilter(int filterId)
        {
            _filterRepository.Delete(f => f.FilterID == filterId);
        }
    }
}
