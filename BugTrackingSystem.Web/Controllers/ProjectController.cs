using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectService _projectService;
        private IUserService _userService;

        public ProjectController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
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
            var projects = _userService.GetUsersProjects(userId);
            return View(projects);
        }

        public ActionResult ProjectUsers()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ProjectBugs()
        {
            return PartialView();
        }
    }
}