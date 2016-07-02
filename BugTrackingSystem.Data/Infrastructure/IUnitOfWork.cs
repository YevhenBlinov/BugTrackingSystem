namespace BugTrackingSystem.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
