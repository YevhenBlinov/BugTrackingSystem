namespace BugTrackingSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BugPriority
    {
        public int BugPriorityID { get; set; }

        [Required]
        [StringLength(20)]
        public string PriorityName { get; set; }
    }
}
