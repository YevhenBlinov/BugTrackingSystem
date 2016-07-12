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

        [HttpPost]
        public void ChangeUser(EditUserFormViewModel user, HttpPostedFileBase image)
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
            throw new NotImplementedException();
        }
        
    }
}