using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        BaseBugViewModel GetBugById(int bugId);

        FullBugViewModel GetFullBugById(int bugId);

        Dictionary<string, string> GetBugAttachmentsByBugId(int bugId);
        
        IEnumerable<CommentViewModel> GetBugCommentsByBugId(int bugId);
        
        IEnumerable<BugViewModel> GetProjectsBugs(int projectId, out int projectsBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        int AddNewBug(BugFormViewModel bugFormViewModel);

        void EditBug(BugEditFormViewModel bugEditFormViewModel);

        void AddBugAttachmentsByBugId(int bugId, Dictionary<string, byte[]> bugAttachmentsDictionary);

        IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, int userId, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle, int? projectId = null);

        IEnumerable<BugViewModel> SearchAllBugsBySubject(string searchRequest, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle, int? projectId = null);

        IEnumerable<BugViewModel> SearchBugsByFilter(int filterId, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        IEnumerable<BugViewModel> SearchBugsByFiltersFields(string search, string[] priority, string[] status,
            int[] projects, int[] users, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle);

        void UpdateBugStatus(int bugId, string status);

        void AddCommentToBug(int bugId, string userName, string comment);

        void DeleteBugAttachment(int bugId, string attachmentName);
    }
}
