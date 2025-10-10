using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        List<ActivityLog> GetLogs();
        void LogAction(int userId, string action, string entityName = null, int? entityId = null);
    }
}
