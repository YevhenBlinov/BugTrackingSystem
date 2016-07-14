using System.Collections.Generic;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectViewModel> GetProjects(out int projectsCount, int currentPage = 1,
            string sortBy = Constants.SortProjectsByTitle);

        ProjectViewModel GetProjectById(int projectId);

        void AddNewProject(string name, string prefix);

        void DeleteProject(int projectId);

        void UpdateProjectName(int projectId, string name);

        IEnumerable<ProjectViewModel> GetAllRunningProjects();

        void PauseAndUnpauseProject(int projectId);

        void RemoveUserFromProject(int projectId, int userId);

        IEnumerable<UserViewModel> GetAllProjectUsers(int projectId);

        IEnumerable<ProjectViewModel> SearchProjectsByName(string searchRequest, out int findedProjectsCount,
            int currentPage = 1, string sortBy = Constants.SortProjectsByTitle);
    }
}
