using BusinessObjects;

namespace Repositories.Interface
{
    public interface IAuditLogRepository
    {
        List<AuditLog> GetAuditLogs(string? keyword, int pageNumber, int pageSize);
        int CountAuditLogs(string? keyword);
        void SaveAuditLog(AuditLog log);
    }
}
