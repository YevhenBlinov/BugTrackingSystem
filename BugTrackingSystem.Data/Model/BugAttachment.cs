using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Data.Model
{
    public partial class BugAttachment
    {
        public int BugAttachmentID { get; set; }

        public int BugID { get; set; }

        [StringLength(1100)]
        public string Attachment { get; set; }

        public virtual Bug Bug { get; set; }
    }
}
