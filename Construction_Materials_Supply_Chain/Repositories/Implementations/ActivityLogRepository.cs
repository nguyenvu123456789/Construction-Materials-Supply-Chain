using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(ScmVlxdContext context) : base(context) { }

        public List<ActivityLog> GetLogs() => _dbSet.ToList();

        public void LogAction(int userId, string action, string entityName = null, int? entityId = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action
            };
            _dbSet.Add(log);
            _context.SaveChanges();
        }
    }
}
