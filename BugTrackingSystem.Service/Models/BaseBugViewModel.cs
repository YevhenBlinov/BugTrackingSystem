namespace BugTrackingSystem.Service.Models
{
    public class BaseBugViewModel
    {
        public int BugId { get; set; }

        public string Priority { get; set; }

        public string ProjectPrefix { get; set; }

        public int Number { get; set; }

        public string Subject { get; set; }

        public string Status { get; set; }
    }
}
