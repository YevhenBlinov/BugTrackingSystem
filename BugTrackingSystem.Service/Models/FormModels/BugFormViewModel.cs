using System;

namespace BugTrackingSystem.Service.Models.FormModels
{
    public class BugFormViewModel
    {
        public int AssignedUserID { get; set; }

        public int ProjectID { get; set; }

        public string Subject { get; set; }

        public int Number { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public byte StatusID { get; set; }

        public byte PriorityID { get; set; }

        public string Description { get; set; }
    }
}
