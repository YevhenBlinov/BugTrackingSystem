using System.Collections.Generic;

namespace BugTrackingSystem.Web.Models
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public IEnumerable<BugViewModel> Bugs { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; }
    }
}