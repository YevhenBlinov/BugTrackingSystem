using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Data.Model
{
    public partial class UserRole
    {
        public int UserRoleID { get; set; }

        [Required]
        [StringLength(20)]
        public string RoleName { get; set; }
    }
}
