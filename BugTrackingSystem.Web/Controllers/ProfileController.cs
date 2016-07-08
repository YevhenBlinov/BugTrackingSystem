using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
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
            return View();
        }
        public ActionResult UserProjects(int userId = 1)
        {
            var projects = _userService.GetUsersProjects(userId);
            return PartialView(projects);
        }

        public ActionResult UserInfo(int userId = 1)
        {
            var user = _userService.GetUserById(userId);
            return PartialView(user);
        }
    }
}