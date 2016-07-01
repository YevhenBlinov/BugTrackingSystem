﻿using BugTrackingSystem.Data.Infrastructure;

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
