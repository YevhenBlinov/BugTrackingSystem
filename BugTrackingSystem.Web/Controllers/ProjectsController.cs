using System.Collections.Generic;
using System.Web.Mvc;

namespace BugTracking.Controllers
{
    public class ProjectsController : Controller
    {
        //
        // GET: /Projects/
        public ActionResult Projects()
        {
            List<Project> list;
            using (AsignarContext ctx = new AsignarContext("AsignarDB"))
            {
                list = ctx.Projects.Select(p => p).ToList();
            }
            return View(list);
        }
    }
}