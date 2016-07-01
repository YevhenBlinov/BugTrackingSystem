using BugTrackingSystem.Data.Infrastructure;

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
