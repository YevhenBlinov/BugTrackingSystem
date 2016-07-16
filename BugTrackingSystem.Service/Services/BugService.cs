using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BugTrackingSystem.AzureService;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Models.FormModels;
using BugPriority = BugTrackingSystem.Service.Models.BugPriority;
using BugStatus = BugTrackingSystem.Service.Models.BugStatus;
using UserRole = BugTrackingSystem.Service.Models.UserRole;

namespace BugTrackingSystem.Service.Services
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IBugAttachmentRepository _bugAttachmentRepository;
        private readonly IMapper _mapper;
        private readonly BlobService _blobService;

        public BugService(IBugRepository bugRepository, IBugAttachmentRepository bugAttachmentRepository)
        {
            _bugRepository = bugRepository;
            _bugAttachmentRepository = bugAttachmentRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                    .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Count))
                    .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Count));
                cfg.CreateMap<Project, ProjectViewModel>();
                cfg.CreateMap<Bug, BaseBugViewModel>();
                cfg.CreateMap<Bug, BugViewModel>()
                    .ForMember(bvm => bvm.AssignedUser, opt => opt.MapFrom(b => b.User))
                    .ForMember(bgm => bgm.Status, opt => opt.MapFrom(b => (BugStatus)b.StatusID))
                    .ForMember(bgm => bgm.Priority, opt => opt.MapFrom(b => (BugPriority)b.PriorityID));
                cfg.CreateMap<Bug, FullBugViewModel>()
                    .ForMember(fbvm => fbvm.AssignedUser, opt => opt.MapFrom(b => b.User))
                    .ForMember(fbvm => fbvm.Status, opt => opt.MapFrom(b => (BugStatus)b.StatusID))
                    .ForMember(fbvm => fbvm.Priority, opt => opt.MapFrom(b => (BugPriority)b.PriorityID))
                    .ForMember(fbvm => fbvm.Comments, opt => opt.Ignore());
                cfg.CreateMap<CommentModel, CommentViewModel>();
                cfg.CreateMap<BugFormViewModel, Bug>()
                .ForMember(b => b.Subject, opt => opt.MapFrom(bfvm => bfvm.Title))
                .ForMember(b => b.ProjectID, opt => opt.MapFrom(bfvm => bfvm.Project))
                .ForMember(b => b.AssignedUserID, opt => opt.MapFrom(bfvm => bfvm.Assignee == 0 ? (int?)null : bfvm.Assignee))
                .ForMember(b => b.PriorityID, opt => opt.MapFrom(bfvm => (byte)((BugPriority)Enum.Parse(typeof(BugPriority), bfvm.Priority))))
                .ForMember(b => b.StatusID, opt => opt.MapFrom(bfvm => (byte)((BugStatus)Enum.Parse(typeof(BugStatus), bfvm.Status))))
                .ForMember(b => b.Description, opt => opt.MapFrom(bfvm => bfvm.Description))
                .ForMember(b => b.BugID, opt => opt.Ignore())
                .ForMember(b => b.BugAttachments, opt => opt.Ignore())
                .ForMember(b => b.Comments, opt => opt.Ignore())
                .ForMember(b => b.Project, opt => opt.Ignore())
                .ForMember(b => b.User, opt => opt.Ignore());
            });

            _mapper = config.CreateMapper();
            _blobService = new BlobService(Constants.UsersPhotosContainerName);
        }

        public BaseBugViewModel GetBugById(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);

            if(bug == null)
                throw new Exception("Sorry, but the bug doesn't exist.");

            var bugModel = _mapper.Map<Bug, BaseBugViewModel>(bug);
            return bugModel;
        }

        public FullBugViewModel GetFullBugById(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);

            if (bug == null)
                throw new Exception("Sorry, but the bug doesn't exist.");

            var fullbugModel = _mapper.Map<Bug, FullBugViewModel>(bug);

            if (bug.AssignedUserID != null)
            {
                fullbugModel.AssignedUser.Photo = _blobService.GetBlobSasUri(fullbugModel.AssignedUser.Photo);
            }

            return fullbugModel;
        }

        public Dictionary<string, string> GetBugAttachmentsByBugId(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);

            if (bug == null)
                throw new Exception("Sorry, but the bug doesn't exist.");

            if (bug.BugAttachments.Count == 0)
                return new Dictionary<string, string>();

            var attachmentBlobService = new BlobService("attachments" + bugId);
            var bugAttachmentsList = bug.BugAttachments.ToDictionary(bugAttachment => bugAttachment.Attachment,
                bugAttachment => attachmentBlobService.GetBlobSasUri(bugAttachment.Attachment));
            return bugAttachmentsList;
        }

        public IEnumerable<CommentViewModel> GetBugCommentsByBugId(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);

            if (bug == null)
                throw new Exception("Sorry, but the bug doesn't exist.");

            if (string.IsNullOrEmpty(bug.Comments)) 
                return new List<CommentViewModel>();

            var commentService = new CommentService();
            var comments = commentService.GetCommentsForBug(bug.BugID);
            return comments;
        }

        public IEnumerable<BugViewModel> GetProjectsBugs(int projectId, out int projectsBugsCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var projectsbugs =
                _bugRepository.GetMany(b => b.ProjectID == projectId);
            projectsBugsCount = projectsbugs.Count();
            projectsbugs = SortHelper.SortBugs(projectsbugs, sortBy);
            projectsbugs = projectsbugs.Skip((currentPage - 1)*Constants.ListPageSize).Take(Constants.ListPageSize);
            var projectbugmodels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(projectsbugs).ToList();

            foreach (var projectBugViewModel in projectbugmodels)
            {
                if (projectBugViewModel.AssignedUser == null)
                    continue;

                projectBugViewModel.AssignedUser.Photo = _blobService.GetBlobSasUri(projectBugViewModel.AssignedUser.Photo);
            }

            return projectbugmodels;
        }

        public int AddNewBug(BugFormViewModel bugFormViewModel)
        {
            var bug = _mapper.Map<BugFormViewModel, Bug>(bugFormViewModel);
            var dateTimeNow = DateTime.Now;
            bug.CreationDate = dateTimeNow;
            bug.ModificationDate = dateTimeNow;
            _bugRepository.Add(bug);
            _bugRepository.Save();

            var addedBugId =
                _bugRepository.Get(
                    b =>
                        b.ProjectID == bug.ProjectID && b.AssignedUserID == bug.AssignedUserID &&
                        b.CreationDate == bug.CreationDate && b.ModificationDate == bug.ModificationDate &&
                        b.PriorityID == bug.PriorityID && b.StatusID == bug.StatusID && b.Description == bug.Description).BugID;

            if (bugFormViewModel.Attachments.Count == 0)
                return addedBugId;

            AddBugAttachmentsByBugId(addedBugId, bugFormViewModel.Attachments);
            return addedBugId;
        }

        public void EditBug(BugEditFormViewModel bugEditFormViewModel)
        {
            var bugToEdit = _bugRepository.GetById(bugEditFormViewModel.BugId);
            bugToEdit.ModificationDate = DateTime.Now;
            bugToEdit.Subject = bugEditFormViewModel.Title;
            bugToEdit.ProjectID = bugEditFormViewModel.Project;
            bugToEdit.AssignedUserID = bugEditFormViewModel.Assignee;
            bugToEdit.PriorityID =
                (byte) ((BugPriority) Enum.Parse(typeof (BugPriority), bugEditFormViewModel.Priority));
            bugToEdit.StatusID = (byte) ((BugStatus) Enum.Parse(typeof (BugStatus), bugEditFormViewModel.Status));
            bugToEdit.Description = bugEditFormViewModel.Description;
            _bugRepository.Update(bugToEdit);
            _bugRepository.Save();

            if (bugEditFormViewModel.Attachments.Count == 0) 
                return;

            AddBugAttachmentsByBugId(bugEditFormViewModel.BugId, bugEditFormViewModel.Attachments);
        }

        public void AddBugAttachmentsByBugId(int bugId, Dictionary<string, byte[]> bugAttachmentsDictionary)
        {
            if(bugAttachmentsDictionary.Count == 0)
                return;

            var blobService = new BlobService("attachments" + bugId);

            foreach (var bugAttachment in bugAttachmentsDictionary)
            {
                blobService.UploadBlobIntoContainerFromByteArray(bugAttachment.Key, bugAttachment.Value);
            }

            foreach (var bugAttachment in bugAttachmentsDictionary)
            {
                _bugAttachmentRepository.Add(new BugAttachment() { Attachment = bugAttachment.Key, BugID = bugId });
            }

            _bugAttachmentRepository.Save();
        }

        public IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, out int findedBugsCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            if (string.IsNullOrEmpty(searchRequest))
            {
                findedBugsCount = 0;
                return new List<BugViewModel>();
            }

            var findedBugs =
                _bugRepository.GetMany(b => b.Project.DeletedOn == null && b.Subject.Contains(searchRequest));
            findedBugsCount = findedBugs.Count();
            findedBugs = SortHelper.SortBugs(findedBugs, sortBy);
            findedBugs = findedBugs.Skip((currentPage - 1)*Constants.ListPageSize).Take(Constants.ListPageSize);
            var findedBugsViewModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(findedBugs).ToList();

            foreach (var projectBugViewModel in findedBugsViewModels)
            {
                if (projectBugViewModel.AssignedUser == null)
                    continue;

                projectBugViewModel.AssignedUser.Photo = _blobService.GetBlobSasUri(projectBugViewModel.AssignedUser.Photo);
            }

            return findedBugsViewModels;
        }

        public void UpdateBugStatus(int bugId, string status)
        {
            var bugToUpdate = _bugRepository.GetById(bugId);

            if (bugToUpdate == null)
                throw new Exception("Sorry, but the bug doesn't exist.");
            
            var updateStatusValue = (BugStatus) Enum.Parse(typeof(BugStatus), status, true);
            bugToUpdate.StatusID = (byte)updateStatusValue;
            _bugRepository.Update(bugToUpdate);
            _bugRepository.Save();
        }

        public void AddCommentToBug(int bugId, string userName, string comment)
        {
            var bug = _bugRepository.GetById(bugId);

            if (bug == null)
                throw new Exception("Sorry, but the bug doesn't exist.");

            var commentService = new CommentService();
            commentService.AddComment(bugId, userName, comment);

            if (!string.IsNullOrEmpty(bug.Comments)) 
                return;

            bug.Comments = bugId.ToString();
            _bugRepository.Update(bug);
            _bugRepository.Save();
        }

        public void DeleteBugAttachment(int bugId, string attachmentName)
        {
            var blobService = new BlobService("attachments" + bugId);
            blobService.DeleteBlobFromContainer(attachmentName);
            _bugAttachmentRepository.Delete(ba => ba.BugID == bugId && ba.Attachment == attachmentName);
            _bugAttachmentRepository.Save();
        }
    }
}
