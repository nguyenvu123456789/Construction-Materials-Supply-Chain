using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        public List<ActivityLog> GetLogs() => ActivityLogDAO.GetLogs();
        public List<ActivityLog> SearchLogs(string keyword) => ActivityLogDAO.SearchLogs(keyword);
        public void LogAction(int userId, string action, string entityName = null, int? entityId = null)
            => ActivityLogDAO.LogAction(userId, action, entityName, entityId);
    }
}
