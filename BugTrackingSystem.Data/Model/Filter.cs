namespace BugTrackingSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        [Column(TypeName = "datetime2")]
        public DateTime? DeletedOn { get; set; }

        public virtual User User { get; set; }
    }
}
