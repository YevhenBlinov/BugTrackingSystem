using System.Web.Mvc;

namespace BugTracking.Controllers
{
    public class TaskController : Controller
    {
        //
        // GET: /Task/
        public ActionResult Task()
        {
            return View();
        }
    }
}