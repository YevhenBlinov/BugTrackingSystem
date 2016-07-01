using BugTrackingSystem.Data.Infrastructure;

namespace BugTrackingSystem.Data.Repositories
{
    public class BugRepository : RepositoryBase<Bug>, IBugRepository
    {
        public BugRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
