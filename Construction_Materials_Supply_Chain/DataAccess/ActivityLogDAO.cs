using BusinessObjects;

namespace DataAccess
{
    public class ActivityLogDAO
    {
        public static void LogAction(int userId, string action, string entityName = null, int? entityId = null)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    var log = new ActivityLog
                    {
                        UserId = userId,
                        Action = action,
                        EntityName = entityName,
                        EntityId = entityId,
                        CreatedAt = DateTime.Now
                    };
                    context.ActivityLogs.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<ActivityLog> SearchLogs(string keyword)
        {
            using (var context = new ScmVlxdContext())
            {
                return context.ActivityLogs
                              .Where(l => l.Action.Contains(keyword)
                                       || l.EntityName.Contains(keyword))
                              .OrderByDescending(l => l.CreatedAt)
                              .ToList();
            }
        }
    }
}
