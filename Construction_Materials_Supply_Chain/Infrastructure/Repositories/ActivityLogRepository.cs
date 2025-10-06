using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(ScmVlxdContext context) : base(context) { }

        public List<ActivityLog> GetLogs()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public void LogAction(int userId, string action, string? entityName = null, int? entityId = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            };
            _dbSet.Add(log);
            _context.SaveChanges();
        }
    }
}
