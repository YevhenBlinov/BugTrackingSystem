using System.Web.Mvc;

namespace BugTracking.Controllers
{
    public class FiltersController : Controller
    {
        //
        // GET: /Filters/
        public ActionResult Filters()
        {
            AsignarContext ctx = new AsignarContext("AsignarDB");
            var filters = ctx.Filters.Select(f => f).ToList();
            return View(filters);
        }
    }
}