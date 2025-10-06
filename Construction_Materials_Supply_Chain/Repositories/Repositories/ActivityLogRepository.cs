using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ActivityLogDAO _dao;

        public ActivityLogRepository(ActivityLogDAO dao)
        {
            _dao = dao;
        }

        public List<ActivityLog> GetLogs() => _dao.GetLogs();
        public List<ActivityLog> SearchLogs(string keyword) => _dao.SearchLogs(keyword);
        public void LogAction(int userId, string action, string entityName = null, int? entityId = null)
            => _dao.LogAction(userId, action, entityName, entityId);

        public List<ActivityLog> GetLogsPaged(string? searchTerm, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
            => _dao.GetLogsPaged(searchTerm, fromDate, toDate, pageNumber, pageSize);

        public int GetTotalLogsCount(string? searchTerm, DateTime? fromDate, DateTime? toDate)
            => _dao.GetTotalLogsCount(searchTerm, fromDate, toDate);
    }
}