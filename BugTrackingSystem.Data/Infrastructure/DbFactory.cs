using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private DBModel _dbContext;

        public DBModel Init()
        {
            return _dbContext ?? (_dbContext = new DBModel());
        }

        protected override void DisposeCore()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }
    }
}
