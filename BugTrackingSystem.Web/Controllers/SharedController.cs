using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models;
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
            var bugsCount = 0;
            var userBugs = _userService.GetUsersBugs(userId, out bugsCount);
            return PartialView(userBugs);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult CreateTask()
        {
            var projects = _projectService.GetAllRunningProjects();
            var usersCount = 0;
            var users = _userService.GetUsers(out usersCount);
            return PartialView(Tuple.Create(projects, users));
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

        public ActionResult ProjectsDropDown(int userId = 0)
        {
            IEnumerable<ProjectViewModel> projects;
            if (userId == 0)
            {
                var projectsCount = 0;
                projects = _projectService.GetProjects(out projectsCount);
            }
            else
            {
                projects = _userService.GetUsersProjects(userId);
            }
            return PartialView(projects);
        }
    }
}