using System.Collections.Generic;

namespace BugTrackingSystem.Service.Models
{
    public class FullBugViewModel : BugViewModel
    {
        public string Description { get; set; }

        public IEnumerable<string> Attachments { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}
