using System.Collections.Generic;
using BugTrackingSystem.Data.Infrastructure;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class BugAttachmentService : IBugAttachmentService
    {
        private readonly IBugAttachmentRepository _bugAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BugAttachmentService(IBugAttachmentRepository bugAttachmentRepository, IUnitOfWork unitOfWork)
        {
            _bugAttachmentRepository = bugAttachmentRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BugAttachment> GetAllBugAttachments()
        {
            var bugAttachments = _bugAttachmentRepository.GetAll();
            return bugAttachments;
        }
    }
}
