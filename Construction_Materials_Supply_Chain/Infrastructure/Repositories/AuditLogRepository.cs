using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

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
