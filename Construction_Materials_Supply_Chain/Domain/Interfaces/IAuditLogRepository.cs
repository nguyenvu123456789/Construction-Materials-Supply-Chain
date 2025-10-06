using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        List<AuditLog> GetAuditLogs();
        void SaveAuditLog(AuditLog log);
    }
}
