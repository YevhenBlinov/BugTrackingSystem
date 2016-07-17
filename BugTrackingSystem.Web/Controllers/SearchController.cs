﻿using System;
using System.Web.Mvc;
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

        public SearchController(IFilterService filterService, IProjectService projectService, IUserService userService)
        {
            _filterService = filterService;
            _projectService = projectService;
            _userService = userService;
        }
        //
        // GET: /Search/
        public ActionResult Search(int filterId = 11, string search = null)
        {
            ViewBag.FilterId = filterId;
            ViewBag.Search = search;
            return View();
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

        public ActionResult SearchResult(int filterId = 0, string search = null)
        {
            return PartialView();
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