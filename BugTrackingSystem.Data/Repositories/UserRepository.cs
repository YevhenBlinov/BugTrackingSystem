using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;

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
