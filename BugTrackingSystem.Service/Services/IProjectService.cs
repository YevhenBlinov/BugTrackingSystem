using System.Collections.Generic;

namespace BugTrackingSystem.Service.Services
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAllProjects();
    }
}
