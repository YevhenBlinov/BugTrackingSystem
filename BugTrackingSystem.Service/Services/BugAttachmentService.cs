using System.Collections.Generic;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class BugAttachmentService : IBugAttachmentService
    {
        private readonly IBugAttachmentRepository _bugAttachmentRepository;

        public BugAttachmentService(IBugAttachmentRepository bugAttachmentRepository)
        {
            _bugAttachmentRepository = bugAttachmentRepository;
        }

        public IEnumerable<BugAttachment> GetAllBugAttachments()
        {
            var bugAttachments = _bugAttachmentRepository.GetAll();
            return bugAttachments;
        }
    }
}
