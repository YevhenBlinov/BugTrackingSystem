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
        private readonly IProjectService _projectService;

        public SharedController(IUserService userService, IProjectService projectService)
        {
            _userService = userService;
            _projectService = projectService;
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

        public ActionResult CreateTask()
        {
            return PartialView();
        }

        public ActionResult CreateFilter()
        {
            return PartialView();
        }

        public void DeleteProject(int projectId)
        {
            _projectService.DeleteProject(projectId);
        }

        public ActionResult DeleteProjectModal(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return PartialView();
        }
    }
}