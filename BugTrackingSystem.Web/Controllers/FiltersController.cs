using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BugTrackingSystem.Service;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class FiltersController : Controller
    {
        private readonly IFilterService _filterService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public FiltersController(IFilterService filterService, IProjectService projectService,IUserService userService)
        {
            _filterService = filterService;
            _projectService = projectService;
            _userService = userService;
        }

        //
        // GET: /Filters/
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult FiltersInfo( string search = null, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            IEnumerable<FilterViewModel> filters;

            var filtersCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                
                filters = _filterService.GetUserFilters(userId, out filtersCount, sortBy:sortBy);
            }
            else
            {
                filters = _filterService.SearchFiltersByTitle(userId, search, out filtersCount, 1, sortBy);
            }
            return PartialView(filters);
        }

        public ActionResult DeleteFilterModal(int filterId)
        {
            ViewBag.FilterId = filterId;
            return PartialView();
        }

        public void DeleteFilter(int filterId)
        {
            _filterService.DeleteFilter(filterId);
        }

        public ActionResult ProjectMultipleSelect()
        {
            int projectsCount = 0;
            var projects = _projectService.GetProjects(out projectsCount);
            return PartialView(projects);
        }

        public ActionResult UserMultipleSelect()
        {
            int usersCount = 0;
            var users = _userService.GetUsers(out usersCount);
            return PartialView(users);
        }

        public ActionResult AddFilter(string title, string search, string[] priority, string[] status, int[] projects,
            int[] users)
        {
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            _filterService.AddFilter(userId, title, search, priority, status, projects, users);
            return RedirectToActionPermanent("FiltersInfo", "Filters");
        }
    }
}