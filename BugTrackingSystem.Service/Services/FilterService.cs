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
                cfg.CreateMap<Filter, FilterViewModel>()
                    .ForMember(fvm => fvm.FilterId, opt => opt.MapFrom(f => f.FilterID))
                    .ForMember(fvm => fvm.Search, opt => opt.MapFrom(f => f.Search))
                    .ForMember(fvm => fvm.Title, opt => opt.MapFrom(f => f.Title))
                    .ForMember(fvm => fvm.Project, opt => opt.MapFrom(f => (f.Project != null) ? ConvertStringToIntArray(f.Project) : null))
                    .ForMember(fvm => fvm.AssignedUser, opt => opt.MapFrom(f => (f.AssignedUser != null) ? ConvertStringToIntArray(f.AssignedUser) : null))
                    .ForMember(fvm => fvm.BugPriority, opt => opt.MapFrom(f => (f.BugPriority != null) ? ConvertStringToStringArray(f.BugPriority) : null))
                    .ForMember(fvm => fvm.BugStatus, opt => opt.MapFrom(f => (f.BugStatus != null) ? ConvertStringToStringArray(f.BugStatus) : null));
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<FilterViewModel> GetUserFilters(int userId, out int filtersCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var filters = _filterRepository.GetMany(f => f.UserID == userId && f.DeletedOn == null);
            filtersCount = filters.Count();
            filters = SortHelper.SortFilters(filters, sortBy);
            filters = filters.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
            var filterModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(filters);
            return filterModels;
        }

        public void AddFilter(int userId, string title, string search, string[] priority, string[] status, int[] projects, int[] users)
        {
            var filter = new Filter
            {
                Title = (!string.IsNullOrEmpty(title)) ? title : null,
                Search = (!string.IsNullOrEmpty(search)) ? search : null,
                BugPriority = (priority != null) ? ConvertStringArrayToString(priority) : null,
                BugStatus = (status != null) ? ConvertStringArrayToString(status) : null,
                Project = (projects != null) ? ConvertIntArrayToString(projects) : null,
                AssignedUser = (users != null) ? ConvertIntArrayToString(users) : null,
                UserID = userId
            };

            _filterRepository.Add(filter);
            _filterRepository.Save();
        }

        private string ConvertStringArrayToString(string[] arrayToConvert)
        {
            var result = string.Join(",", arrayToConvert);
            return result;
        }

        private string ConvertIntArrayToString(int[] arrayToConvert)
        {
            var result = string.Join(",", arrayToConvert);
            return result;
        }

        private string[] ConvertStringToStringArray(string stringToConvert)
        {
            var result = stringToConvert.Split(',');
            return result;
        }

        private int[] ConvertStringToIntArray(string stringToConvert)
        {
            var stringArray = stringToConvert.Split(',');
            var result = stringArray.Select(n => Convert.ToInt32(n)).ToArray();
            return result;
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

        public IEnumerable<FilterViewModel> SearchFiltersByTitle(int userId, string searchRequest, out int findedFiltersCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            if (string.IsNullOrEmpty(searchRequest))
            {
                findedFiltersCount = 0;
                return new List<FilterViewModel>();
            }

            var findedFilters =
                _filterRepository.GetMany(
                    f => f.UserID == userId && f.DeletedOn == null && f.Title.Contains(searchRequest));
            findedFiltersCount = findedFilters.Count();
            findedFilters = SortHelper.SortFilters(findedFilters, sortBy);
            findedFilters = findedFilters.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
            var findedFiltersViewModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(findedFilters);
            return findedFiltersViewModels;
        }
    }
}
