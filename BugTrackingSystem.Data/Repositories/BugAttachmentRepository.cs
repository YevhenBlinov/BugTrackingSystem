using BugTrackingSystem.Data.Infrastructure;

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
