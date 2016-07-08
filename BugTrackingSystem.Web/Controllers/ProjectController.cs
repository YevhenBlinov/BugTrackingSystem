using System.Linq;
using System.Web.Mvc;
using BugTrackingSystem.Service.Services;
using PagedList;

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

        public ActionResult Projects(int? page, int userId = 1)
        {
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            ViewBag.AllProjects = _userService.GetUsersProjects(userId).Count();

            var projects = _userService.GetUsersProjects(userId).OrderBy(x => x.ProjectId).ToPagedList(pageNumber, pageSize);
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