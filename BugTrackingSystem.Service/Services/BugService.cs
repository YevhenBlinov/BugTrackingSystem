using System;
using System.Collections.Generic;
using BugTrackingSystem.Data.Repositories;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Models;
using AutoMapper;

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
                cfg.CreateMap<User, UserViewModel>();
                cfg.CreateMap<Project, ProjectViewModel>();
                cfg.CreateMap<Bug, BugViewModel>();
            });

            _mapper = config.CreateMapper();
        }

        public IEnumerable<BugViewModel> GetAllBugs()
        {
            var bugs = _bugRepository.GetAll();
            var bugModels = _mapper.Map<IEnumerable<Bug>, IEnumerable<BugViewModel>>(bugs);
            return bugModels;
        }

        public BugViewModel GetBugById(int bugId)
        {
            var bug = _bugRepository.GetById(bugId);
            var bugModel = _mapper.Map<Bug, BugViewModel>(bug);
            return bugModel;
        }
    }
}
