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
    public class SearchController : Controller
    {
        private readonly IFilterService _filterService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IBugService _bugService;

        public SearchController(IFilterService filterService, IProjectService projectService, IUserService userService, IBugService bugService)
        {
            _filterService = filterService;
            _projectService = projectService;
            _userService = userService;
            _bugService = bugService;
        }
        //
        // GET: /Search/
        public ActionResult Search(int filterId = 0, string search = null)
        {
            ViewBag.FilterId = filterId;
            ViewBag.Search = search;
            return View();
        }

        public ActionResult SearchFromNav(string search)
        {
            return RedirectToActionPermanent("Search", "Search", new {search = search});
        }

        public ActionResult SearchByKeyWord(string search)
        {
            return RedirectToActionPermanent("Search", "Search", new {search = search});
        }

        public ActionResult SearchFilter(int filterId = 0, string search = null)
        {
            ViewBag.Search = search;
            if (filterId != 0)
            {
                var filter = _filterService.GetAdvancedFilterById(filterId);
                return PartialView(filter);
            }
            return PartialView(null);
        }

        public ActionResult SearchResult(int filterId = 0, string search = null, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var bugsCount = 0;
            ViewBag.CurrentPage = currentPage;
            ViewBag.FilterId = filterId;
            IEnumerable<BugViewModel> bugs;
            double pagesCount;
            if (filterId != 0)
            {
               bugs = _bugService.SearchBugsByFilter(filterId, out bugsCount, currentPage, sortBy);
               pagesCount = Convert.ToDouble(bugsCount) / Convert.ToDouble(Constants.StickerPageSize);
               ViewBag.PagesCount = Math.Ceiling(pagesCount);
               ViewBag.TaskCount = bugsCount;
               return PartialView(bugs);
            }
            bugs = _bugService.SearchAllBugsBySubject(search, out bugsCount, currentPage, sortBy);
            pagesCount = Convert.ToDouble(bugsCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.TaskCount = bugsCount;
            ViewBag.FiltersCount = bugsCount;  
            return PartialView(bugs);
        }

        public ActionResult SearchResultByParams(string search, string[] priority, string[] status, int[] projects, int[] users, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var bugsCount = 0;
            var bugs = _bugService.SearchBugsByFiltersFields(search, priority, status, projects, users,
                out bugsCount, currentPage, sortBy);
            var pagesCount = Convert.ToDouble(bugsCount) / Convert.ToDouble(Constants.StickerPageSize);
            ViewBag.PagesCount = Math.Ceiling(pagesCount);
            ViewBag.FiltersCount = bugsCount;
            ViewBag.CurrentPage = currentPage;
            return PartialView("SearchResult", bugs);
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

        public ActionResult SearchFilters()
        {
            var filtersCount = 0;
            var userId = Convert.ToInt32(Session["UserId"].ToString());
            var filters = _filterService.GetUserFilters(userId, out filtersCount);
            return PartialView(filters);
        }
    }
}