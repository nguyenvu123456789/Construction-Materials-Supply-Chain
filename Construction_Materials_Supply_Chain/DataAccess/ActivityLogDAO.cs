using BusinessObjects;

namespace DataAccess
{
    public class ActivityLogDAO : BaseDAO
    {
        public ActivityLogDAO(ScmVlxdContext context) : base(context) { }

        public List<ActivityLog> GetLogs()
        {
            return Context.ActivityLogs
                          .OrderByDescending(l => l.CreatedAt)
                          .ToList();
        }

        public List<ActivityLog> SearchLogs(string keyword)
        {
            return Context.ActivityLogs
                          .Where(l => l.Action.Contains(keyword)
                                   || l.EntityName.Contains(keyword))
                          .OrderByDescending(l => l.CreatedAt)
                          .ToList();
        }

        public void LogAction(int userId, string action, string entityName = null, int? entityId = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                CreatedAt = DateTime.Now
            };
            Context.ActivityLogs.Add(log);
            Context.SaveChanges();
        }
    }
}
