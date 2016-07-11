namespace BugTrackingSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BugStatuses")]
    public partial class BugStatus
    {
        public int BugStatusID { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusName { get; set; }
    }
}
