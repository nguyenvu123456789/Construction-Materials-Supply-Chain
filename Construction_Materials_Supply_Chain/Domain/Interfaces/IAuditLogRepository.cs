using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        IQueryable<AuditLog> QueryWithUser();
    }
}
