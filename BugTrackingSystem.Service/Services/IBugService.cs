using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        BaseBugViewModel GetBugById(int bugId);

        FullBugViewModel GetFullBugById(int bugId);

        IEnumerable<BugViewModel> GetProjectsBugs(int projectId, out int projectsBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        void AddNewBug(BugFormViewModel bugFormViewModel);

        IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        void UpdateBugStatus(int bugId, string status);
    }
}
