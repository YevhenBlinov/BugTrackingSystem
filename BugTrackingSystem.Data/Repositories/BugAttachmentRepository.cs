using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Repositories
{
    class BugAttachmentRepository : RepositoryBase<BugAttachment>, IBugAttachmentRepository
    {
        public BugAttachmentRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
