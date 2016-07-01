using System.Web.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
    }
}