using System.Collections.Generic;
using System.Web.Mvc;
using BugTrackingSystem.Service;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class FiltersController : Controller
    {
        IFilterService _filterService;
        IProjectService _projectService;

        public FiltersController(IFilterService filterService, IProjectService projectService)
        {
            _filterService = filterService;
            _projectService = projectService;
        }

        //
        // GET: /Filters/
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult FiltersInfo(int userId = 1, string search = null, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
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
    }
}