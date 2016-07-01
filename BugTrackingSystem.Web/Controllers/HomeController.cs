using System.Web.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            AsignarContext ctx = new AsignarContext("AsignarDB");

            var bugs = ctx.Bugs.ToList();
            return View(bugs);
        }
        public ActionResult MyProjects()
        {
            AsignarContext ctx = new AsignarContext("AsignarDB");
            var projects = ctx.Projects.Select(b => b).ToList();
            return PartialView(projects);
        }
    }
}