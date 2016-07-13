using System.Collections.Generic;
using System.Linq;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Service
{
    public static class SortHelper
    {
        public static IEnumerable<Bug> SortBugs(IEnumerable<Bug> bugsToSort, string sortBy)
        {
            switch (sortBy)
            {
                case Constants.SortBugsByTitle:
                    {
                        return bugsToSort.OrderBy(b => b.Subject).ToList();
                    }
                case Constants.SortBugsByProject:
                    {
                        return bugsToSort.OrderBy(b => b.ProjectID).ToList();
                    }
                case Constants.SortBugsByAssigneedUser:
                    {
                        return bugsToSort.OrderBy(b => b.AssignedUserID).ToList();
                    }
                case Constants.SortBugsByStatus:
                    {
                        return bugsToSort.OrderBy(b => b.StatusID).ToList();
                    }
                case Constants.SortBugsByPriority:
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
    }
}
