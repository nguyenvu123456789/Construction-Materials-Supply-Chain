using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AuditLogDAO : BaseDAO
    {
        public AuditLogDAO(ScmVlxdContext context) : base(context) { }

        public List<AuditLog> GetAuditLogs(string? keyword, int pageNumber, int pageSize)
        {
            var query = Context.AuditLogs
                               .Include(l => l.User)
                               .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(l =>
                    l.EntityName.Contains(keyword) ||
                    l.Action.Contains(keyword) ||
                    (l.Changes != null && l.Changes.Contains(keyword)));
            }

            return query.OrderByDescending(l => l.CreatedAt)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int CountAuditLogs(string? keyword)
        {
            var query = Context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(l =>
                    l.EntityName.Contains(keyword) ||
                    l.Action.Contains(keyword) ||
                    (l.Changes != null && l.Changes.Contains(keyword)));
            }

            return query.Count();
        }

        public void SaveAuditLog(AuditLog log)
        {
            Context.AuditLogs.Add(log);
            Context.SaveChanges();
        }
    }
}
