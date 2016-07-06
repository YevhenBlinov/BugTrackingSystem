using System;
using System.Collections.Generic;
using AutoMapper;
using BugTrackingSystem.AzureService;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Service.Models;
using BugPriority = BugTrackingSystem.Service.Models.BugPriority;
using BugStatus = BugTrackingSystem.Service.Models.BugStatus;
using UserRole = BugTrackingSystem.Service.Models.UserRole;

namespace BugTrackingSystem.Service.Services
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IMapper _mapper;

        public BugService(IBugRepository bugRepository)
        {
            _bugRepository = bugRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(uvm => uvm.Role, opt => opt.MapFrom(u => (UserRole)u.UserRoleID));
                cfg.CreateMap<Project, ProjectViewModel>();
                cfg.CreateMap<Bug, BaseBugViewModel>();
                cfg.CreateMap<Bug, BugViewModel>()
                    .ForMember(bvm => bvm.AssignedUser, opt => opt.MapFrom(b => b.User))
                    .ForMember(bgm => bgm.Status, opt => opt.MapFrom(b => Enum.GetName(typeof(BugStatus), b.StatusID)))
                    .ForMember(bgm => bgm.Priority, opt => opt.MapFrom(b => Enum.GetName(typeof(BugPriority), b.PriorityID)));
                cfg.CreateMap<Bug, FullBugViewModel>()
                    .ForMember(fbvm => fbvm.AssignedUser, opt => opt.MapFrom(b => b.User))
                    .ForMember(fbvm => fbvm.Status, opt => opt.MapFrom(b => Enum.GetName(typeof(BugStatus), b.StatusID)))
                    .ForMember(fbvm => fbvm.Priority, opt => opt.MapFrom(b => Enum.GetName(typeof(BugPriority), b.PriorityID)))
                    .ForMember(fbvm => fbvm.Comments, opt => opt.Ignore());
                cfg.CreateMap<CommentModel, CommentViewModel>();

            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<BaseBugViewModel> GetAllBugs()
        {
            var bugs = _bugRepository.GetAll();
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BaseBugViewModel>>(bugs);
            return bugModels;
        }

        public BaseBugViewModel GetBugById(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);
            var bugModel = _mapper.Map<Bug, BaseBugViewModel>(bug);
            return bugModel;
        }

        public FullBugViewModel GetFullBugById(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);
            var fullbugModel = _mapper.Map<Bug, FullBugViewModel>(bug);
            var tableService = new TableService();
            var comments = tableService.RetrieveAllCommentsForBug(bugId.ToString());
            fullbugModel.Comments = _mapper.Map<List<CommentModel>, List<CommentViewModel>>(comments); ;
            return fullbugModel;
        }

        public IEnumerable<BugViewModel> GetAllProjectsBugs(int projectId)
        {
            var allprojectsbugs = _bugRepository.GetMany(b => b.ProjectID == projectId);
            var allprojectbugmodels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(allprojectsbugs);
            return allprojectbugmodels;
        }
    }
}
