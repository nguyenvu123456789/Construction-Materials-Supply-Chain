using System.Collections.Generic;
using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interface
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        List<AuditLog> GetAuditLogs();
        void SaveAuditLog(AuditLog log);
    }
}
