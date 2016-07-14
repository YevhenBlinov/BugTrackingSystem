using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Service.Services;
using PagedList;

namespace BugTrackingSystem.Web.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectService _projectService;
        private IUserService _userService;
        private IBugService _bugService;

        public ProjectController(IProjectService projectService, IUserService userService, IBugService bugService)
        {
            _projectService = projectService;
            _userService = userService;
            _bugService = bugService;
        }
        //
        // GET: /Project/
        [HttpGet]
        public ActionResult Project(int projectId)
        {
            var project = _projectService.GetProjectById(projectId);
            return View(project);
        }

        public ActionResult Projects()
        {
            return View();
        }

        public ActionResult ProjectsInfo(int userId = 1)
        {
            var projectsCount = 0;
            var projects = _projectService.GetProjects(out projectsCount);
            return PartialView(projects);
        }

        public ActionResult ProjectUsers(int projectId)
        {
            var users = _projectService.GetAllProjectUsers(projectId);
            ViewBag.ProjectId = projectId;
            return PartialView(users);
        }

        [HttpGet]
        public ActionResult ProjectBugs(int projectId)
        {
            var bugsCount = 0;
            var bugs = _bugService.GetProjectsBugs(projectId, out bugsCount);
            return PartialView(bugs);
        }

        public void AddProject(string name, string prefix)
        {
            _projectService.AddNewProject(name, prefix);
        }

        public void DeleteProject(int projectId)
        {
            _projectService.DeleteProject(projectId);
        }

        public void EditProject(int projectId, string name)
        {
            _projectService.UpdateProjectName(projectId, name);
        }

        public void PauseProject(int projectId)
        {
            _projectService.PauseAndUnpauseProject(projectId);
        }

        [HttpPost]
        public void DeleteUserFromProject(int projectId, int userId)
        {
            _projectService.RemoveUserFromProject(projectId, userId);
        }
    }
}