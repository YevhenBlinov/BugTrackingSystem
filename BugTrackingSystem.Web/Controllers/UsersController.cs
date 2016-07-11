using System;
using System.Web.Mvc;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        //
        // GET: /Users/
        public ActionResult Users()
        {
            var users = _userService.GetAllUsers();
            return View(users);
        }

        public ActionResult DeleteUserModal(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }

        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void AddUser(string userData)
        {
            //_userService.AddUser(firstName, lastName, email, password, role);
        }
    }
}