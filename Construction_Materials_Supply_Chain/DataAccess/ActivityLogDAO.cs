using BusinessObjects;

namespace DataAccess
{
    public class ActivityLogDAO
    {
        public static List<ActivityLog> GetLogs()
        {
            var list = new List<ActivityLog>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.ActivityLogs
                                  .OrderByDescending(l => l.CreatedAt)
                                  .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }

        public static List<ActivityLog> SearchLogs(string keyword)
        {
            var list = new List<ActivityLog>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.ActivityLogs
                                  .Where(l => l.Action.Contains(keyword)
                                           || l.EntityName.Contains(keyword))
                                  .OrderByDescending(l => l.CreatedAt)
                                  .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }

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
    }
}
