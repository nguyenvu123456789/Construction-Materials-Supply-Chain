using Domain;
using Infrastructure.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
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
