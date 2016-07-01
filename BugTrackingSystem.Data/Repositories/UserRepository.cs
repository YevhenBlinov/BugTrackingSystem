using BugTrackingSystem.Data.Infrastructure;

namespace BugTrackingSystem.Data.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
