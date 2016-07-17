using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Service;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class SharedController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IFilterService _filterService;

        public SharedController(IUserService userService, IProjectService projectService, IFilterService filterService)
        {
            _userService = userService;
            _projectService = projectService;
            _filterService = filterService;
        }
        // GET: Shared
        public ActionResult UserBugs(int page = 1)
        {
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            var bugsCount = 0;
            var userBugs = _userService.GetUsersBugs(userId, out bugsCount, page);
            double pagesCount = Convert.ToDouble(bugsCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.TaskCount = bugsCount;
            ViewBag.CurrentPage = page;
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


        [CustomAuthorize(Roles = "Administrator")]
        public void DeleteProject(int projectId)
        {
            _projectService.DeleteProject(projectId);
        }

        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult DeleteProjectModal(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return PartialView();
        }

        public ActionResult UsersDropDown(int projectId = 0)
        {
            IEnumerable<UserViewModel> users;
            if (projectId == 0)
            {
                var usersCount = 0;
                users = _userService.GetUsers(out usersCount);
            }
            else
            {
                users = _projectService.GetAllProjectUsers(projectId);
            }
            return PartialView(users);
        }

        public ActionResult ProjectsDropDown()
        {
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            IEnumerable<ProjectViewModel> projects;
            if (userId == 0)
            {
                projects = _projectService.GetAllRunningProjects();
            }
            else
            {
                projects = _userService.GetUsersProjects(userId);
            }
            return PartialView(projects);
        }
    }
}