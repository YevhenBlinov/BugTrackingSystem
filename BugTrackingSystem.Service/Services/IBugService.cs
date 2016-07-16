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

        IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, UserRole userRole, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle, int? projectId = null);

        void UpdateBugStatus(int bugId, string status);

        void AddCommentToBug(int bugId, string userName, string comment);

        void DeleteBugAttachment(int bugId, string attachmentName);
    }
}
