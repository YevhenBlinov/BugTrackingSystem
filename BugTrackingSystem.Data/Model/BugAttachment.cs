namespace BugTrackingSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BugAttachment
    {
        public int BugAttachmentID { get; set; }

        public int BugID { get; set; }

        [StringLength(1100)]
        public string Attachment { get; set; }

        public virtual Bug Bug { get; set; }
    }
}
