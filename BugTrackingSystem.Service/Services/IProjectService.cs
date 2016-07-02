using System.Collections.Generic;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Service.Services
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAllProjects();

        IEnumerable<Project> GetAllUsersProject();
    }
}
