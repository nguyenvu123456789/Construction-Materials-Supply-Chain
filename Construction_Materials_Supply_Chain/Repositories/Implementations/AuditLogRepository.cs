using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ScmVlxdContext context) : base(context) { }

        public List<AuditLog> GetAuditLogs() => _dbSet.ToList();

        public void SaveAuditLog(AuditLog log)
        {
            _dbSet.Add(log);
            _context.SaveChanges();
        }
    }
}
