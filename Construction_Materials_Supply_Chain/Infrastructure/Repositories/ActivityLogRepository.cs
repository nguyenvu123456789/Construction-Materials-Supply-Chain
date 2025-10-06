using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

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
