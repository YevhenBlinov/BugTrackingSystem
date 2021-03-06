﻿using System;
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
                    .ForMember(fvm => fvm.Project, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToIntArray(f.Project)))
                    .ForMember(fvm => fvm.AssignedUser, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToIntArray(f.AssignedUser)))
                    .ForMember(fvm => fvm.BugPriority, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToStringArray(f.BugPriority)))
                    .ForMember(fvm => fvm.BugStatus, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToStringArray(f.BugStatus)));
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
            var filterModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(filters);
            filterModels = SortHelper.SortFilters(filterModels, sortBy);
            filterModels = filterModels.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
            return filterModels;
        }

        public IEnumerable<FilterViewModel> GetAllUserFilters(int userId)
        {
            var filters = _filterRepository.GetMany(f => f.UserID == userId && f.DeletedOn == null);
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
            var projectsIntArray = ConvertHelper.ConvertStringToIntArray(projects);
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
            var usersIntArray = ConvertHelper.ConvertStringToIntArray(users);
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
                BugPriority = (priority != null) ? ConvertHelper.ConvertStringArrayToString(priority) : null,
                BugStatus = (status != null) ? ConvertHelper.ConvertStringArrayToString(status) : null,
                Project = (projects != null) ? ConvertHelper.ConvertIntArrayToString(projects) : null,
                AssignedUser = (users != null) ? ConvertHelper.ConvertIntArrayToString(users) : null,
                UserID = userId
            };

            _filterRepository.Add(filter);
            _filterRepository.Save();
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
            var findedFiltersViewModels = _mapper.Map<IEnumerable<Filter>, IEnumerable<FilterViewModel>>(findedFilters);
            findedFiltersViewModels = SortHelper.SortFilters(findedFiltersViewModels, sortBy);
            findedFiltersViewModels = findedFiltersViewModels.Skip((currentPage - 1) * Constants.StickerPageSize).Take(Constants.StickerPageSize);
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
