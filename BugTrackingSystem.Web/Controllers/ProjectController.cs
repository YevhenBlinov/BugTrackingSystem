using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTracking.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        //
        // GET: /Project/
        [HttpGet]
        public ActionResult Project(int projectId)
        {
            var projects = _projectService.GetProjectById(projectId);
            return View(projects);
        }

        public ActionResult Projects()
        {

            return View();
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