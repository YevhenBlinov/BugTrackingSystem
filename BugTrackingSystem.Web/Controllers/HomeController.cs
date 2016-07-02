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

            return View();
        }
        public ActionResult MyProjects()
        {

            return PartialView();
        }
    }
}