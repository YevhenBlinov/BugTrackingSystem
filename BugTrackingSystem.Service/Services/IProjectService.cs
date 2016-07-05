﻿using System.Collections.Generic;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectViewModel> GetAllProjects();

        ProjectViewModel GetProjectById(int projectId);
    }
}
