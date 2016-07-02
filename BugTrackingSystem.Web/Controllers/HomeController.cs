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

        public ActionResult Login()
        {

            return View();
        }
        public ActionResult ForgotPassword()
        {

            return View();
        }

        public ActionResult ResetPassword()
        {

            return View();
        }

    }
}