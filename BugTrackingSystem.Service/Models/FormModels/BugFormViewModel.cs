using System;
using System.Collections.Generic;


namespace BugTrackingSystem.Service.Models.FormModels
{
    public class BugFormViewModel
    {

        public string Title { get; set; }

        public int Project { get; set; }

        public int Assignee { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public Dictionary<string, byte[]> Attachments { get; set; }
    }
}
