using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        IEnumerable<BaseBugViewModel> GetAllBugs();

        BaseBugViewModel GetBugById(int bugId);

        FullBugViewModel GetFullBugById(int bugId);

        IEnumerable<BugViewModel> GetAllProjectsBugs(int projectId);
    }
}
