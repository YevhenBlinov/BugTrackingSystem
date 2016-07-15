using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BugTrackingSystem.Service;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Service.Services;
using PagedList;

namespace BugTrackingSystem.Web.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectService _projectService;
        private IUserService _userService;
        private IBugService _bugService;

        public ProjectController(IProjectService projectService, IUserService userService, IBugService bugService)
        {
            _projectService = projectService;
            _userService = userService;
            _bugService = bugService;
        }
        //
        // GET: /Project/
        [HttpGet]
        public ActionResult Project(int projectId)
        {
            var project = _projectService.GetProjectById(projectId);
            return View(project);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectsInfo( string sortBy = Constants.SortProjectsByTitle, int userId = 1, string search = null, int page = 1)
        {
            var projectsCount = 0;
            IEnumerable<ProjectViewModel> projects;
            if (string.IsNullOrEmpty(search))
            {
                projects = _projectService.GetProjects(out projectsCount, page, sortBy);
            }
            else
            {
                projects = _projectService.SearchProjectsByName(search, out projectsCount, page, sortBy);
            }

            double pagesCount = Convert.ToDouble(projectsCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.ProjectsCount = projectsCount;
            ViewBag.CurrentPage = page;

            return PartialView(projects);
        }

        public ActionResult ProjectUsers(int projectId)
        {
            var users = _projectService.GetAllProjectUsers(projectId);
            ViewBag.ProjectId = projectId;
            return PartialView(users);
        }

        [HttpGet]
        public ActionResult ProjectBugs(int projectId, string search = null, string sortBy = Constants.SortBugsOrFiltersByTitle, int page = 1)
        {
            IEnumerable<BugViewModel> bugs;
            var bugsCount = 0;
            if(string.IsNullOrEmpty(search))
            {
                bugs = _bugService.GetProjectsBugs(projectId, out bugsCount, page, sortBy);
            }
            else
            {
                bugs = _bugService.SearchBugsBySubject(search, out bugsCount, page, sortBy);
            }
            double pagesCount = Convert.ToDouble(bugsCount) / Convert.ToDouble(Constants.ListPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.TaskCount = bugsCount;
            ViewBag.CurrentPage = page;
            ViewBag.ProjectId = projectId;
            return PartialView(bugs);
        }

        public void AddProject(string name, string prefix)
        {
            _projectService.AddNewProject(name, prefix);
        }

        public void DeleteProject(int projectId)
        {
            _projectService.DeleteProject(projectId);
        }

        public void EditProject(int projectId, string name)
        {
            _projectService.UpdateProjectName(projectId, name);
        }

        public void PauseProject(int projectId)
        {
            _projectService.PauseAndUnpauseProject(projectId);
        }

        [HttpPost]
        public void DeleteUserFromProject(int projectId, int userId)
        {
            _projectService.RemoveUserFromProject(projectId, userId);
        }

        public ActionResult AddUserToProject(int projectId)
        {
            var users = _userService.GetNotAssignedToProjectUsers(projectId);
            ViewBag.ProjectId = projectId;
            return PartialView(users);
        }

        public void AddUsers(string userIds, int projectId)
        {
            _projectService.AddUsersToProject(projectId, userIds);
        }
    }
}