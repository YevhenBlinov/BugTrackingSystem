using System.Collections.Generic;

namespace BugTrackingSystem.Web.Models
{
    public class UserViewModel
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public ICollection<BugViewModel> Bugs { get; set; }

        public ICollection<ProjectViewModel> Projects { get; set; }
    }
}