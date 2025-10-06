using Domain.Models;

namespace Application.Interfaces
{
    public interface IActivityLogService
    {
        List<ActivityLog> GetAll();
        void LogAction(int userId, string action, string? entityName = null, int? entityId = null);

        List<ActivityLog> GetFiltered(string? searchTerm, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, out int totalCount);
    }
}
