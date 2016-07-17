namespace BugTrackingSystem.Service.Models
{
    public class FilterViewModel
    {
        public int FilterId { get; set; }

        public string Title { get; set; }

        public int[] Project { get; set; }

        public int[] AssignedUser { get; set; }

        public string Search { get; set; }

        public string[] BugStatus { get; set; }

        public string[] BugPriority { get; set; }
    }
}
