using Domain;
using Infrastructure.Base;

namespace Infrastructure.Interface
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        List<AuditLog> GetAuditLogs();
        void SaveAuditLog(AuditLog log);
    }
}
