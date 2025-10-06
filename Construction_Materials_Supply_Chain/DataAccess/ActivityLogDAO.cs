using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ActivityLogDAO : BaseDAO
    {
        public ActivityLogDAO(ScmVlxdContext context) : base(context) { }

        public List<ActivityLog> GetLogs()
        {
            return Context.ActivityLogs
                          .Include(l => l.User)
                          .OrderByDescending(l => l.CreatedAt)
                          .ToList();
        }

        public List<ActivityLog> SearchLogs(string keyword)
        {
            return Context.ActivityLogs
                          .Include(l => l.User)
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

        public List<ActivityLog> GetLogsPaged(string? searchTerm, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var query = Context.ActivityLogs
                              .Include(l => l.User)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (int.TryParse(searchTerm, out int userId))
                {
                    query = query.Where(l => l.UserId == userId);
                }
                else
                {
                    query = query.Where(l => l.User != null && l.User.UserName.Contains(searchTerm));
                }
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt <= toDate.Value);
            }

            return query.OrderByDescending(l => l.CreatedAt)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalLogsCount(string? searchTerm, DateTime? fromDate, DateTime? toDate)
        {
            var query = Context.ActivityLogs
                              .Include(l => l.User)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (int.TryParse(searchTerm, out int userId))
                {
                    query = query.Where(l => l.UserId == userId);
                }
                else
                {
                    query = query.Where(l => l.User != null && l.User.UserName.Contains(searchTerm));
                }
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt <= toDate.Value);
            }

            return query.Count();
        }
    }
}