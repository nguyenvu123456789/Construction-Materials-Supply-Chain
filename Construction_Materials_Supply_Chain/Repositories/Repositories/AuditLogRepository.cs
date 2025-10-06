using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AuditLogDAO _dao;

        public AuditLogRepository(AuditLogDAO dao)
        {
            _dao = dao;
        }

        public List<AuditLog> GetAuditLogs(string? keyword, int pageNumber, int pageSize) =>
            _dao.GetAuditLogs(keyword, pageNumber, pageSize);

        public int CountAuditLogs(string? keyword) =>
            _dao.CountAuditLogs(keyword);

        public void SaveAuditLog(AuditLog log) => _dao.SaveAuditLog(log);
    }
}
