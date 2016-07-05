namespace BugTrackingSystem.Service.Models
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public int BugsCount { get; set; }

        public int UsersCount { get; set; }
    }
}
