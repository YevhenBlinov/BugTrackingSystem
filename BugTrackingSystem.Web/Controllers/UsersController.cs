using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

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