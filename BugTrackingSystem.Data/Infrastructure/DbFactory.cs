using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private DBModel _dbContext;

        public DBModel DbContext
        {
            get { return _dbContext ?? (_dbContext = Init()); }
        }

        public DBModel Init()
        {
            return _dbContext ?? (_dbContext = new DBModel());
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
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
