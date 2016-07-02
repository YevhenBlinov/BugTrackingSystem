using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Data.Model
{
    public partial class Filter
    {
        public int FilterID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Project { get; set; }

        public string AssignedUser { get; set; }

        [StringLength(200)]
        public string Search { get; set; }

        public string BugStatus { get; set; }

        public string BugPriority { get; set; }

        public virtual User User { get; set; }
    }
}
