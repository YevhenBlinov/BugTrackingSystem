using System.Collections.Generic;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugAttachmentService
    {
        IEnumerable<BugAttachment> GetAllBugAttachments();
    }
}
