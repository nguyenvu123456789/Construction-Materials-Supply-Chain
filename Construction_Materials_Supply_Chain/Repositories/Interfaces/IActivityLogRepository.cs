using System.Collections.Generic;
using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interface
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        List<ActivityLog> GetLogs();
        void LogAction(int userId, string action, string entityName = null, int? entityId = null);
    }
}
