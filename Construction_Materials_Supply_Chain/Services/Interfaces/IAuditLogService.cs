using BusinessObjects;

namespace Services.Interfaces
{
    public interface IAuditLogService
    {
        List<AuditLog> GetFiltered(string? keyword, int pageNumber, int pageSize, out int totalCount);
        void Save(AuditLog log);
    }
}
