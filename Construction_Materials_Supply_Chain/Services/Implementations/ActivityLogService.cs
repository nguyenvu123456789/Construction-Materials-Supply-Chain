using Application.Interfaces;
using Domain;
using Infrastructure.Interface;

namespace Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _repo;

        public ActivityLogService(IActivityLogRepository repo)
        {
            _repo = repo;
        }

        public List<ActivityLog> GetAll() => _repo.GetLogs();

        public void LogAction(int userId, string action, string? entityName = null, int? entityId = null)
        {
            _repo.LogAction(userId, action, entityName, entityId);
        }

        public List<ActivityLog> GetFiltered(string? searchTerm, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _repo.GetLogs().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(x => (x.Action ?? "").Contains(searchTerm));

            if (fromDate.HasValue) query = query.Where(x => x.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.CreatedAt <= toDate.Value);

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }
    }
}
