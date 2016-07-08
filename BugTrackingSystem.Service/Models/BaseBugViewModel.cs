namespace BugTrackingSystem.Service.Models
{
    public class BaseBugViewModel
    {
        public int BugId { get; set; }

        public BugPriority Priority { get; set; }

        public string ProjectPrefix { get; set; }

        public int Number { get; set; }

        public string Subject { get; set; }

        public BugStatus Status { get; set; }
    }
}
