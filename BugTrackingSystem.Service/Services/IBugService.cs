using System.Collections.Generic;

namespace BugTrackingSystem.Service.Services
{
    public interface IBugService
    {
        IEnumerable<Bug> GetAllBugs();
    }
}
