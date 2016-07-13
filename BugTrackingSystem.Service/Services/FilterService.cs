using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<FilterViewModel> GetUserFilters(int userId, int currentPage = 1)
        {
            var filters =
                _filterRepository.GetMany(f => f.UserID == userId && f.DeletedOn == null)
                    .Skip((currentPage - 1)*Constants.PageSize)
                    .Take(Constants.PageSize);
            var filterModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(filters);
            return filterModels;
        }

        public int GetAllUserFiltersCount(int userId)
        {
            var filtersCount = _filterRepository.GetMany(f => f.UserID == userId && f.DeletedOn == null).Count();
            return filtersCount;
        }

        public void DeleteFilter(int filterId)
        {
            var filter = _filterRepository.GetById(filterId);

            if(filter == null)
                throw new Exception("Sorry, but the filter doesn't exist.");

            filter.DeletedOn = DateTime.Now;
            _filterRepository.Update(filter);
            _filterRepository.Save();
        }

        public IEnumerable<FilterViewModel> SearchFiltersByTitle(string searchRequest, int currentPage = 1)
        {
            if(string.IsNullOrEmpty(searchRequest))
                return new List<FilterViewModel>();

            var findedFilters = _filterRepository.GetMany(f => f.DeletedOn == null && f.Title.Contains(searchRequest))
                .Skip((currentPage - 1)*Constants.PageSize)
                .Take(Constants.PageSize);
            var findedFiltersViewModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(findedFilters);
            return findedFiltersViewModels;
        }

        public int GetFindedFiltersByTitleCount(string searchRequest)
        {
            if(string.IsNullOrEmpty(searchRequest))
                return 0;

            var findedFiltersCount =
                _filterRepository.GetMany(f => f.DeletedOn == null && f.Title.Contains(searchRequest)).Count();
            return findedFiltersCount;
        }
    }
}
