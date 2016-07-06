using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IFilterService
    {
        IEnumerable<FilterViewModel> GetAllUserFilters(int userId);


    }
}
