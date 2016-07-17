using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IFilterService
    {
        IEnumerable<FilterViewModel> GetUserFilters(int userId, out int filtersCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        void AddFilter(int userId, string title, string search, string[] priority, string[] status, int[] projects, int[] users);

        void DeleteFilter(int filterId);

        IEnumerable<FilterViewModel> SearchFiltersByTitle(int userId, string searchRequest, out int findedFiltersCount,
            int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle);

        AdvancedFilterViewModel GetAdvancedFilterById(int filterId);
    }
}
