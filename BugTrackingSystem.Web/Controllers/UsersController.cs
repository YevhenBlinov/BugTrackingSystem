using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using Microsoft.Ajax.Utilities;

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

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users(string search = null)
        {
            IEnumerable<UserViewModel> users;
            var usersCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                users = _userService.GetUsers(out usersCount);  
            }
            else
            {
                users = _userService.SearchUsersByFirstNameAndSecondName(search, out usersCount);
            }
            
            return PartialView(users);
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
            _userService.ChangeUserPassword(userId, password);
        }

        [HttpPost]
        public ActionResult EditUser(EditUserFormViewModel user, HttpPostedFileBase image)
        {
            if (image != null)
            {
                using (Stream inputStream = image.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    byte[] data = memoryStream.ToArray();
                    user.Photo = data;
                }
            }

            _userService.EditUserInformation(user);
            return RedirectToActionPermanent("Index", "Users");
        }
        
    }
}