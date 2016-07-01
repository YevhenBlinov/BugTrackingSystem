using System;

namespace BugTrackingSystem.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        StoreEntities Init();
    }
}
