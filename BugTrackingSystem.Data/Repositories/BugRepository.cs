using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;

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
