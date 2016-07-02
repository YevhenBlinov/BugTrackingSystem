using System;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        DBModel Init();
    }
}
