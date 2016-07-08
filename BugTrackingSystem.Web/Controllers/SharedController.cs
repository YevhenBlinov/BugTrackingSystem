using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class SharedController : Controller
    {
        private readonly IUserService _userService;

        public SharedController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: Shared
        public ActionResult UserBugs(int userId = 1)
        {
            var userBugs = _userService.GetUsersBugs(userId);
            return PartialView(userBugs);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}