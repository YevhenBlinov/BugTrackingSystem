using System.Collections.Generic;
using System.Linq;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service
{
    public static class SortHelper
    {
        public static IEnumerable<Bug> SortBugs(IEnumerable<Bug> bugsToSort, string sortBy)
        {
            switch (sortBy)
            {
                case Constants.SortBugsOrFiltersByTitle:
                    {
                        return bugsToSort.OrderBy(b => b.Subject).ToList();
                    }
                case Constants.SortBugsOrFiltersByProject:
                    {
                        return bugsToSort.OrderBy(b => b.ProjectID).ToList();
                    }
                case Constants.SortBugsOrFiltersByAssigneedUser:
                    {
                        return
                            bugsToSort.Where(b => b.User != null)
                                .OrderBy(b => b.User.LastName)
                                .Concat(bugsToSort.Where(b => b.User == null))
                                .ToList();
                    }
                case Constants.SortBugsOrFiltersByStatus:
                    {
                        return bugsToSort.OrderBy(b => b.StatusID).ToList();
                    }
                case Constants.SortBugsOrFiltersByPriority:
                    {
                        return bugsToSort.OrderBy(b => b.PriorityID).ToList();
                    }
                default:
                    {
                        return bugsToSort;
                    }
            }
        }

        public static List<User> SortUsers(IEnumerable<User> usersToSort, string sortBy)
        {
            switch (sortBy)
            {
                case Constants.SortUsersByName:
                    {
                        return usersToSort.OrderBy(u => u.FirstName).ToList();
                    }
                case Constants.SortUsersBySurname:
                    {
                        return usersToSort.OrderBy(u => u.LastName).ToList();
                    }
                default:
                    {
                        return usersToSort.ToList();
                    }
            }
        }

        public static IEnumerable<Project> SortProjects(IEnumerable<Project> projectsToSort, string sortBy)
        {
            switch (sortBy)
            {
                case Constants.SortProjectsByTitle:
                    {
                        return projectsToSort.OrderBy(p => p.Name).ToList();
                    }
                case Constants.SortProjectsByPrefix:
                    {
                        return projectsToSort.OrderBy(p => p.Prefix).ToList();
                    }
                default:
                    {
                        return projectsToSort;
                    }
            }
        }

        public static IEnumerable<FilterViewModel> SortFilters(IEnumerable<FilterViewModel> filtersToSort, string sortBy)
        {
            switch (sortBy)
            {
                case Constants.SortBugsOrFiltersByTitle:
                    {
                        return filtersToSort.OrderBy(f => f.Title).ToList();
                    }
                case Constants.SortBugsOrFiltersByProject:
                    {
                        return filtersToSort.OrderBy(f => f.Project).ToList();
                    }
                case Constants.SortBugsOrFiltersByAssigneedUser:
                    {
                        return filtersToSort.OrderBy(f => f.AssignedUser).ToList();
                    }
                case Constants.SortBugsOrFiltersByStatus:
                    {
                        return filtersToSort.OrderBy(f => f.BugStatus).ToList();
                    }
                case Constants.SortBugsOrFiltersByPriority:
                    {
                        return filtersToSort.OrderBy(f => f.BugPriority).ToList();
                    }
                default:
                    {
                        return filtersToSort;
                    }
            }
        }

        public static IEnumerable<CommentViewModel> SortComments(IEnumerable<CommentViewModel> comments)
        {
            return comments.OrderByDescending(c => c.Date);
        }
    }
}
