using System.Linq;
using System.Web.Mvc;
using BugTrackingSystem.Service.Services;
using PagedList;

namespace BugTrackingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;

        public HomeController(IUserService userService, IProjectService projectService)
        {
            _userService = userService;
            _projectService = projectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard(int? page)
        {
            //var user = _userService.GetUserById(1);
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            ViewBag.AllProjects = _userService.GetUsersBugs(1).Count();
            var userBugs = _userService.GetUsersBugs(1).OrderBy(x => x.BugId).ToPagedList(pageNumber, pageSize);


            return View(userBugs);
        }
        public ActionResult MyProjects()
        {
            var userProjects = _userService.GetUsersProjects(1);
            return PartialView(userProjects);
        }

        //public ActionResult Login()
        //{

        //    return View();
        //}
        //public ActionResult ForgotPassword()
        //{

        //    return View();
        //}

        //public ActionResult ResetPassword()
        //{

        //    return View();
        //}

    }
}