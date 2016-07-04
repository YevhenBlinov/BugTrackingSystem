﻿using System.Collections.Generic;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Data.Repositories;

namespace BugTrackingSystem.Service.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;

        public FilterService(IFilterRepository filterRepository)
        {
            _filterRepository = filterRepository;
        }
        public IEnumerable<Filter> GetAllFilters()
        {
            var filter = _filterRepository.GetAll();
            return filter;
        }
    }
}
