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

        public ActionResult Projects(int userId = 1)
        {
            var projects = _projectService.GetAllProjects();
            return View(projects);
        }

        public ActionResult ProjectUsers()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ProjectBugs(int projectId)
        {
            var bugs = _bugService.GetAllProjectsBugs(projectId);
            return PartialView(bugs);
        }

        public void AddProject(string name, string prefix)
        {
            _projectService.AddNewProject(name, prefix);
            RedirectToAction("Projects", "Project");
        }

        public void DeleteProject(int projectId)
        {
            _projectService.DeleteProject(projectId);
            RedirectToAction("Projects", "Project");
        }
    }
}