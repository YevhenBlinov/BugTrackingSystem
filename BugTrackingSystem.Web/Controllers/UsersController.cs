using System.Web.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/
        public ActionResult Users()
        {
            return View();
        }
    }
}