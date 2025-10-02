using BusinessObjects;

namespace Repositories.Interface
{
    public interface IActivityLogRepository
    {
        List<ActivityLog> GetLogs();
        List<ActivityLog> SearchLogs(string keyword);
        void LogAction(int userId, string action, string entityName = null, int? entityId = null);

        List<ActivityLog> GetLogsPaged(string? keyword, int pageNumber, int pageSize);
        int GetTotalLogsCount(string? keyword);
    }
}
