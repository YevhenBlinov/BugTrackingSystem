using System.Collections.Generic;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

namespace BugTrackingSystem.Service.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectViewModel> GetAllProjects();

        ProjectViewModel GetProjectById(int projectId);

        void AddNewProject(string name, string prefix);

        void DeleteProject(int projectId);
    }
}
