using System;

namespace BugTrackingSystem.Service.Models
{
    public class BugViewModel
    {
        public int BugId { get; set; }

        public string Subject { get; set; }

        public int Number { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string Description { get; set; }
    }
}
