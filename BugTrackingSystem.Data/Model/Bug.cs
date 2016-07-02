using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BugTrackingSystem.Data.Model
{
    public partial class Bug
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bug()
        {
            BugAttachments = new HashSet<BugAttachment>();
        }

        public int BugID { get; set; }

        public int ProjectID { get; set; }

        public int? AssignedUserID { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        public int Number { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ModificationDate { get; set; }

        public byte StatusID { get; set; }

        public byte PriorityID { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(1100)]
        public string Comments { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BugAttachment> BugAttachments { get; set; }

        public virtual User User { get; set; }

        public virtual Project Project { get; set; }
    }
}
