using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        IEnumerable<BugViewModel> GetAllBugs();

        BugViewModel GetBugById(int bugId);
    }
}
