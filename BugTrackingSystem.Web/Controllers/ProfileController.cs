using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class ProfileController : Controller
    {

        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: Profile
        public ActionResult Index()
        {
            var userId = _userService.GetUserByEmail(User.Identity.Name).UserId;
            ViewBag.UserId = userId;
            return View();
        }
        public ActionResult UserProjects()
        {
            var userId = _userService.GetUserByEmail(User.Identity.Name).UserId;
            var projects = _userService.GetUsersProjects(userId);
            ViewBag.UserId = userId;
            return PartialView(projects);
        }

        public ActionResult UserInfo()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            return PartialView(user);
        }
    }
}