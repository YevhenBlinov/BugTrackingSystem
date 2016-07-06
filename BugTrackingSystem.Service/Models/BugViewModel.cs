using System;

namespace BugTrackingSystem.Service.Models
{
    public class BugViewModel : BaseBugViewModel
    {
        public string ProjectName { get; set; }

        public UserViewModel AssignedUser { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }
    }
}
