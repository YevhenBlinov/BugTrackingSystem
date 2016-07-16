using System.Web.Mvc;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        public ActionResult Search()
        {
            return View();
        }
    }
}