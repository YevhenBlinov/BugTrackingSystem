using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Repositories
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
