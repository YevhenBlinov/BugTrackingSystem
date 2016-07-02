using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackingSystem.Data.Model
{
    [Table("BugStatuses")]
    public partial class BugStatus
    {
        public int BugStatusID { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusName { get; set; }
    }
}
