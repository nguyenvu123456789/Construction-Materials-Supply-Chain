using BusinessObjects;
using DataAccess;
using Repositories.Interface;

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

    public List<ActivityLog> GetLogsPaged(string? keyword, int pageNumber, int pageSize)
            => _dao.GetLogsPaged(keyword, pageNumber, pageSize);

    public int GetTotalLogsCount(string? keyword)
        => _dao.GetTotalLogsCount(keyword);
}
