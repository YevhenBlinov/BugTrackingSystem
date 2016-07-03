using System;

namespace BugTrackingSystem.Web.Models
{
    public class BugViewModel
    {
        public int BugID { get; set; }

        public int ProjectID { get; set; }

        public int? AssignedUserID { get; set; }

        public string Subject { get; set; }

        public int Number { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Status { get; set; }

        public byte Priority { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public UserViewModel User { get; set; }

        public ProjectViewModel Project { get; set; }
    }
}