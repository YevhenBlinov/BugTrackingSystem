using System.Collections.Generic;

namespace BugTrackingSystem.Service.Services
{
    public interface IFilterService
    {
        IEnumerable<Filter> GetAllFilters();
    }
}
