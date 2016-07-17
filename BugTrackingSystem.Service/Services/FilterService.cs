using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FilterService(IFilterRepository filterRepository, IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _filterRepository = filterRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Filter, AdvancedFilterViewModel>()
                    .ForMember(fvm => fvm.FilterId, opt => opt.MapFrom(f => f.FilterID))
                    .ForMember(fvm => fvm.Search, opt => opt.MapFrom(f => f.Search))
                    .ForMember(fvm => fvm.Title, opt => opt.MapFrom(f => f.Title))
                    .ForMember(fvm => fvm.Project, opt => opt.MapFrom(f => ConvertStringToIntArray(f.Project)))
                    .ForMember(fvm => fvm.AssignedUser, opt => opt.MapFrom(f => ConvertStringToIntArray(f.AssignedUser)))
                    .ForMember(fvm => fvm.BugPriority, opt => opt.MapFrom(f => ConvertStringToStringArray(f.BugPriority)))
                    .ForMember(fvm => fvm.BugStatus, opt => opt.MapFrom(f => ConvertStringToStringArray(f.BugStatus)));
                cfg.CreateMap<Filter, FilterViewModel>()
                    .ForMember(fvm => fvm.BugPriority, opt => opt.MapFrom(f => AddSpacesToSentence(f.BugPriority)))
                    .ForMember(fvm => fvm.BugStatus, opt => opt.MapFrom(f => AddSpacesToSentence(f.BugStatus)))
                    .ForMember(fvm => fvm.AssignedUser, opt => opt.MapFrom(f => GetUsersStringByIds(f.AssignedUser)))
                    .ForMember(fvm => fvm.Project, opt => opt.MapFrom(f => GetProjectsStringByIds(f.Project)));
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
        
        private string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' ');
                }

                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        private string GetProjectsStringByIds(string projects)
        {
            if (string.IsNullOrWhiteSpace(projects))
                return null;

            var newText = new StringBuilder();
            var projectsIntArray = ConvertStringToIntArray(projects);
            var projectsIntArrayLength = projectsIntArray.Length;

            for (var i = 0; i < projectsIntArrayLength; i++)
            {
                var project = _projectRepository.GetById(projectsIntArray[i]);
                newText.Append((i + 1) != projectsIntArrayLength ? project.Name + ", " : project.Name);
            }

            return newText.ToString();
        }

        private string GetUsersStringByIds(string users)
        {
            if (string.IsNullOrWhiteSpace(users))
                return null;

            var newText = new StringBuilder();
            var usersIntArray = ConvertStringToIntArray(users);
            var usersIntArrayLength = usersIntArray.Length;

            for (var i = 0; i < usersIntArrayLength; i++)
            {
                var user = _userRepository.GetById(usersIntArray[i]);
                newText.Append((i + 1) != usersIntArrayLength
                    ? user.FirstName + " " + user.LastName + ", "
                    : user.FirstName + " " + user.LastName);
            }

            return newText.ToString();
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
            if (string.IsNullOrEmpty(stringToConvert))
                return null;

            var result = stringToConvert.Split(',');
            return result;
        }

        private int[] ConvertStringToIntArray(string stringToConvert)
        {
            if (string.IsNullOrEmpty(stringToConvert))
                return null;

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

        public AdvancedFilterViewModel GetAdvancedFilterById(int filterId)
        {
            var filter = _filterRepository.GetById(filterId);

            if(filter == null || filter.DeletedOn != null)
                throw new Exception("Sorry, but the filter doesn't exist.");

            var advancedFilter = _mapper.Map<Filter, AdvancedFilterViewModel>(filter);
            return advancedFilter;
        }
    }
}
