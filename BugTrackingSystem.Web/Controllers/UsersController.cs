using System;
using System.Web.Mvc;
using System.Web.Services;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;

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
            _userService.DeleteUser(userId);
        }

        [WebMethod()]
        public void AddUser(UserFormViewModel userModel)
        {
            _userService.AddUser(userModel);
        }

        public void ChangeUserPassword(int userId, string password)
        {
            throw new NotImplementedException();
        }

        public void ChangeUserInfo(UserViewModel user)
        {
            throw new NotImplementedException();
        }
    }
}