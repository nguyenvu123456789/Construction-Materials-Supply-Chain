using Domain;
using Domain.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
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
