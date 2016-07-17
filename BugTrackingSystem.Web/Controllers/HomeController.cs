using System;
using System.Web.Mvc;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

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
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            var userProjects = _userService.GetUsersProjects(userId);
            return PartialView(userProjects);
        }


    }
}