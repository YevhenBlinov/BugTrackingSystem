using System.Collections.Generic;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugAttachmentService
    {
        IEnumerable<BugAttachment> GetAllBugAttachments();
    }
}
