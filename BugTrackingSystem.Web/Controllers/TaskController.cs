using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly IBugService _bugService;

        public TaskController(IBugService bugService)
        {
            _bugService = bugService;
        }
        //
        // GET: /Task/
        public ActionResult Task(int bugId = 1)
        {
            var info = _bugService.GetFullBugById(bugId);
            return View(info);
        }
    }
}