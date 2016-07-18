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

        public ActionResult FiltersInfo( string search = null, string sortBy = Constants.SortBugsOrFiltersByTitle, int page = 1)
        {
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            IEnumerable<FilterViewModel> filters;

            var filtersCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                
                filters = _filterService.GetUserFilters(userId, out filtersCount, page, sortBy);
            }
            else
            {
                filters = _filterService.SearchFiltersByTitle(userId, search, out filtersCount, page, sortBy);
            }
            double pagesCount = Convert.ToDouble(filtersCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.FiltersCount = filtersCount;
            ViewBag.CurrentPage = page;
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
            IEnumerable<ProjectViewModel> projects;
            if (Session["Role"].ToString() == "User")
            {
                projects = _projectService.GetAllRunningProjects();
            }
            else
            {
                projects = _projectService.GetProjects();
            }            
            return PartialView(projects);
        }

        public ActionResult UserMultipleSelect()
        {
            var users = _userService.GetUsers();
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