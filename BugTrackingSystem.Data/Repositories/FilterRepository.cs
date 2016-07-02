using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Repositories
{
    public class FilterRepository : RepositoryBase<Filter>, IFilterRepository
    {
        public FilterRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
