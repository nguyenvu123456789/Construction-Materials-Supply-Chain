using Application.Interfaces;
using Domain.Models;
using Domain.Interface;

namespace Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogService(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public List<AuditLog> GetFiltered(string? keyword, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _repo.GetAuditLogs().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => (x.Action ?? "").Contains(keyword) || (x.EntityName ?? "").Contains(keyword));

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public void Save(AuditLog log) => _repo.SaveAuditLog(log);
    }
}
