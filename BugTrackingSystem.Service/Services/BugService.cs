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
        private readonly IFilterRepository _filterRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly BlobService _blobService;

        public BugService(IBugRepository bugRepository, IBugAttachmentRepository bugAttachmentRepository, IFilterRepository filterRepository, IProjectRepository projectRepository)
        {
            _bugRepository = bugRepository;
            _bugAttachmentRepository = bugAttachmentRepository;
            _filterRepository = filterRepository;
            _projectRepository = projectRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID))
                    .ForMember(uvm => uvm.ProjectsCount, opt => opt.MapFrom(u => u.Projects.Count))
                    .ForMember(uvm => uvm.BugsCount, opt => opt.MapFrom(u => u.Bugs.Count));
                cfg.CreateMap<Project, ProjectViewModel>()
                    .ForMember(pvm => pvm.UsersCount, opt => opt.MapFrom(p => p.Users.Count(u => u.DeletedOn == null)))
                    .ForMember(pvm => pvm.BugsCount,
                        opt => opt.MapFrom(p => p.Bugs.Count(b => b.AssignedUserID == null || b.User.DeletedOn == null)));
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
                cfg.CreateMap<Filter, AdvancedFilterViewModel>()
                    .ForMember(fvm => fvm.FilterId, opt => opt.MapFrom(f => f.FilterID))
                    .ForMember(fvm => fvm.Search, opt => opt.MapFrom(f => f.Search))
                    .ForMember(fvm => fvm.Title, opt => opt.MapFrom(f => f.Title))
                    .ForMember(fvm => fvm.Project, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToIntArray(f.Project)))
                    .ForMember(fvm => fvm.AssignedUser, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToIntArray(f.AssignedUser)))
                    .ForMember(fvm => fvm.BugPriority, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToStringArray(f.BugPriority)))
                    .ForMember(fvm => fvm.BugStatus, opt => opt.MapFrom(f => ConvertHelper.ConvertStringToStringArray(f.BugStatus)));
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
                _bugRepository.GetMany(b => b.ProjectID == projectId && (b.AssignedUserID == null || b.User.DeletedOn == null));
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

            if (bugFormViewModel.Attachments == null)
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
            bugToEdit.AssignedUserID = bugEditFormViewModel.Assignee != 0 ? bugEditFormViewModel.Assignee : (int?) null;
            bugToEdit.PriorityID =
                (byte) ((BugPriority) Enum.Parse(typeof (BugPriority), bugEditFormViewModel.Priority));
            bugToEdit.StatusID = (byte) ((BugStatus) Enum.Parse(typeof (BugStatus), bugEditFormViewModel.Status));
            bugToEdit.Description = bugEditFormViewModel.Description;
            _bugRepository.Update(bugToEdit);
            _bugRepository.Save();

            if (bugEditFormViewModel.Attachments == null) 
                return;

            AddBugAttachmentsByBugId(bugEditFormViewModel.BugId, bugEditFormViewModel.Attachments);
        }

        public void AddBugAttachmentsByBugId(int bugId, Dictionary<string, byte[]> bugAttachmentsDictionary)
        {
            if(bugAttachmentsDictionary == null)
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

            var bug = _bugRepository.GetById(bugId);
            bug.ModificationDate = DateTime.Now;
            _bugRepository.Update(bug);
            _bugRepository.Save();
        }

        public IEnumerable<BugViewModel> SearchBugsBySubject(string searchRequest, UserRole userRole, out int findedBugsCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle, int? projectId = null)
        {
            if (string.IsNullOrEmpty(searchRequest))
            {
                findedBugsCount = 0;
                return new List<BugViewModel>();
            }

            var findedBugs = new List<Bug>() as IEnumerable<Bug>;

            switch (userRole)
            {
                    case UserRole.User:
                    findedBugs = projectId == null
                        ? _bugRepository.GetMany(b => b.Project.DeletedOn == null && b.Subject.Contains(searchRequest))
                            .Where(b => b.Project.IsPaused == false)
                        : _bugRepository.GetMany(
                            b =>
                                b.ProjectID == projectId && b.Project.DeletedOn == null &&
                                b.Subject.Contains(searchRequest))
                            .Where(b => b.Project.IsPaused == false);
                    break;
                    case UserRole.Administrator:
                    findedBugs = projectId == null
                        ? _bugRepository.GetMany(b => b.Project.DeletedOn == null && b.Subject.Contains(searchRequest))
                        : _bugRepository.GetMany(
                            b =>
                                b.ProjectID == projectId && b.Project.DeletedOn == null &&
                                b.Subject.Contains(searchRequest));
                    break;
            }

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

        public IEnumerable<BugViewModel> SearchBugsByFilter(int filterId, string userRole, out int findedBugsCount, int currentPage = 1,
            string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var filter = _filterRepository.GetById(filterId);

            if (filter == null || filter.DeletedOn != null)
                throw new Exception("Sorry, but the filter doesn't exist.");

            var advancedFilter = _mapper.Map<Filter, AdvancedFilterViewModel>(filter);
            var findedBugs = AdvancedSearch(advancedFilter, userRole, out findedBugsCount, currentPage, sortBy);
            return findedBugs;
        }

        public IEnumerable<BugViewModel> SearchBugsByFiltersFields(string search, string[] priority, string[] status, int[] projects, int[] users,
            string userRole, out int findedBugsCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var advancedFilter = new AdvancedFilterViewModel()
            {
                Project = projects,
                AssignedUser = users,
                BugPriority = priority,
                BugStatus = status,
                Search = search
            };

            var findedBugs = AdvancedSearch(advancedFilter, userRole, out findedBugsCount, currentPage, sortBy);
            return findedBugs;
        }

        private IEnumerable<BugViewModel> AdvancedSearch(AdvancedFilterViewModel advancedFilter, string userRole,
            out int findedBugsCount, int currentPage = 1, string sortBy = Constants.SortBugsOrFiltersByTitle)
        {
            var allBugsBySearch = new List<Bug>();
            if (!string.IsNullOrEmpty(advancedFilter.Search))
            {
                allBugsBySearch =
                    _bugRepository.GetMany(b => b.Project.DeletedOn == null && b.Subject.Contains(advancedFilter.Search))
                        .ToList();
            }

            var allBugsByProjects = new List<Bug>();
            if (advancedFilter.Project.Length != 0)
            {
                foreach (var projectId in advancedFilter.Project)
                {
                    allBugsByProjects.AddRange(_bugRepository.GetMany(b => b.Project.DeletedOn == null && b.Project.ProjectID == projectId));
                }
            }

            var allBugsByUsers = new List<Bug>();
            if (advancedFilter.AssignedUser.Length != 0)
            {
                foreach (var userId in advancedFilter.AssignedUser)
                {
                    allBugsByUsers.AddRange(_bugRepository.GetMany(b => b.Project.DeletedOn == null && b.AssignedUserID == userId));
                }
            }

            var allBugsByStatus = new List<Bug>();
            if (advancedFilter.BugStatus.Length != 0)
            {
                foreach (var bugStatusName in advancedFilter.BugStatus)
                {
                    var bugStatusId = (byte)((BugStatus)Enum.Parse(typeof(BugStatus), bugStatusName));
                    allBugsByStatus.AddRange(
                        _bugRepository.GetMany(b => b.Project.DeletedOn == null && b.StatusID == bugStatusId));
                }
            }

            var allBugsByPriority = new List<Bug>();
            if (advancedFilter.BugPriority.Length != 0)
            {
                foreach (var bugPriorityName in advancedFilter.BugPriority)
                {
                    var bugPriorityId = (byte)((BugPriority)Enum.Parse(typeof(BugPriority), bugPriorityName));
                    allBugsByPriority.AddRange(_bugRepository.GetMany(b => b.Project.DeletedOn == null && b.PriorityID == bugPriorityId));
                }
            }

            var findedBugs = allBugsBySearch;
            if (allBugsByProjects.Count != 0 && findedBugs.Count != 0)
            {
                findedBugs = findedBugs.Intersect(allBugsByProjects).ToList();
            }

            if (allBugsByUsers.Count != 0 && findedBugs.Count != 0)
            {
                findedBugs = findedBugs.Intersect(allBugsByUsers).ToList();
            }

            if (allBugsByStatus.Count != 0 && findedBugs.Count != 0)
            {
                findedBugs = findedBugs.Intersect(allBugsByStatus).ToList();
            }

            if (allBugsByPriority.Count != 0 && findedBugs.Count != 0)
            {
                findedBugs = findedBugs.Intersect(allBugsByPriority).ToList();
            }

            var parsedUserRole = ((UserRole) Enum.Parse(typeof (UserRole), userRole));

            if (parsedUserRole == UserRole.User)
            {
                findedBugs = findedBugs.Where(b => b.Project.IsPaused == false).ToList();
            }

            if (findedBugs.Count != 0)
            {
                findedBugsCount = findedBugs.Count();
                findedBugs = SortHelper.SortBugs(findedBugs, sortBy).ToList();
                findedBugs = findedBugs.Skip((currentPage - 1) * Constants.ListPageSize).Take(Constants.ListPageSize).ToList();
                var findedBugsViewModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(findedBugs).ToList();

                foreach (var projectBugViewModel in findedBugsViewModels)
                {
                    if (projectBugViewModel.AssignedUser == null)
                        continue;

                    projectBugViewModel.AssignedUser.Photo = _blobService.GetBlobSasUri(projectBugViewModel.AssignedUser.Photo);
                }

                return findedBugsViewModels;
            }

            findedBugsCount = 0;
            return new List<BugViewModel>();
        }

        public void UpdateBugStatus(int bugId, string status)
        {
            var bugToUpdate = _bugRepository.GetById(bugId);

            if (bugToUpdate == null)
                throw new Exception("Sorry, but the bug doesn't exist.");
            
            var updateStatusValue = (BugStatus) Enum.Parse(typeof(BugStatus), status, true);
            bugToUpdate.StatusID = (byte)updateStatusValue;
            bugToUpdate.ModificationDate = DateTime.Now;
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

            var bug = _bugRepository.GetById(bugId);
            bug.ModificationDate = DateTime.Now;
            _bugRepository.Update(bug);
            _bugRepository.Save();
        }
    }
}
