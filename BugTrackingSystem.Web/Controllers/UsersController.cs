using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using BugTrackingSystem.Service;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Web.Filters;
using Microsoft.Ajax.Utilities;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
   
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        //
        // GET: /Users/
        [CustomAuthorize(Roles = "Administrator")]
        [Route("Users")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Users(string sortBy = Constants.SortUsersByName, string search = null, int page = 1)
        {
            IEnumerable<UserViewModel> users;
            var usersCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                users = _userService.GetUsers(out usersCount, page, sortBy);
            }
            else
            {
                users = _userService.SearchUsersByFirstNameAndSecondName(search, out usersCount, page, sortBy);
            }
            double pagesCount = Convert.ToDouble(usersCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.UsersCount = usersCount;
            ViewBag.CurrentPage = page;
            return PartialView(users);
        }
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult DeleteUserModal(int userId, int taskCount)
        {
            ViewBag.UserId = userId;
            ViewBag.TaskCount = taskCount;
            return PartialView();
        }
        [CustomAuthorize(Roles = "Administrator")]
        public void DeleteUser(int userId)
        {
            _userService.DeleteUser(userId);
        }

        [WebMethod()]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult AddUser(UserFormViewModel userModel)
        {
            _userService.AddUser(userModel);
            return RedirectToActionPermanent("Index", "Users");
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

            var editedUser = _userService.GetUserByEmail(user.Email);
            if (user.UserId.ToString() == Session["UserId"].ToString())
            {
                Session["FirstName"] = editedUser.FirstName;
                Session["LastName"] = editedUser.LastName;
                Session["Photo"] = editedUser.Photo;
            }
            return RedirectToActionPermanent("Index", "Profile", new { userId = user.UserId });
        }

    }
}