using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Data.Model
{
    public partial class BugPriority
    {
        public int BugPriorityID { get; set; }

        [Required]
        [StringLength(20)]
        public string PriorityName { get; set; }
    }
}
