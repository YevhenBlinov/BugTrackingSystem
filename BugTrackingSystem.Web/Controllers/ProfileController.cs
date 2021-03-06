﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Models;
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
        [Route("Profile/{userId?}")]
        public ActionResult Index(int? userId)
        {
            if (userId == null)
            {
                userId = Convert.ToInt32(Session["UserId"].ToString());
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("Error", "Shared");
            }

            ViewBag.UserId = userId;
            return View();
        }
        public ActionResult UserProjects(int? userId)
        {
            IEnumerable<ProjectViewModel> projects;

            if (userId == null || User.IsInRole("User"))
            {
                userId = Convert.ToInt32(Session["UserId"].ToString());
                projects = _userService.GetUsersProjects((int)userId);
            }
            else
            {
                projects = _userService.GetUsersProjects((int)userId);
            }

            ViewBag.UserId = userId;
            return PartialView(projects);
        }

        public ActionResult UserInfo(int userId)
        {
            BugTrackingSystem.Service.Models.UserViewModel user;
            if (User.IsInRole("Administrator"))
                user = _userService.GetUserById(userId);
            else
                user = _userService.GetUserByEmail(User.Identity.Name);
            return PartialView(user);
        }
    }
}