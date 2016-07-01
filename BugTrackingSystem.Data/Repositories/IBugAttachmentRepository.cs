using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;

namespace BugTrackingSystem.Data.Repositories
{
    public interface IBugAttachmentRepository : IRepository<BugAttachment>
    {
        IEnumerable<BugAttachment> GetAllBugAttachments();
    }
}
