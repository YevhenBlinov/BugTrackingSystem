using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        IEnumerable<BaseBugViewModel> GetAllBugs();

        BaseBugViewModel GetBugById(int bugId);

        FullBugViewModel GetFullBugById(int bugId);

        IEnumerable<BugViewModel> GetProjectsBugs(int projectId, int currentPage = 1, string sortBy = Constants.SortBugsByTitle);

        int GetAllProjectsBugsCount(int projectId);

        void AddNewBug(BugFormViewModel bugFormViewModel);

        IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, int currentPage = 1, string sortBy = Constants.SortBugsByTitle);

        int GetFindedBugsBySubjectCount(string searchRequest);
    }
}
