using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        List<AuditLog> GetAuditLogs();
        void SaveAuditLog(AuditLog log);
    }
}
