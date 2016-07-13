using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IFilterService
    {
        IEnumerable<FilterViewModel> GetUserFilters(int userId, int currentPage = 1);

        int GetAllUserFiltersCount(int userId);

        void DeleteFilter(int filterId);

        IEnumerable<FilterViewModel> SearchFiltersByTitle(string searchRequest, int currentPage = 1);

        int GetFindedFiltersByTitleCount(string searchRequest);
    }
}
