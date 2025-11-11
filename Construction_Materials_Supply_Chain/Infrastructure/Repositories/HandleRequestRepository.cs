using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class HandleRequestRepository : GenericRepository<HandleRequest>, IHandleRequestRepository
    {
        public HandleRequestRepository(ScmVlxdContext context) : base(context) { }

        // 🔹 Lấy tất cả handle request theo loại và id, luôn include HandledByNavigation
        public List<HandleRequest> GetByRequest(string requestType, int requestId)
        {
            return _dbSet
                .Where(r => r.RequestType == requestType && r.RequestId == requestId)
                .Include(r => r.HandledByNavigation) // include User
                .OrderByDescending(r => r.HandledAt) // order mới nhất
                .ToList();
        }

        public bool Exists(string requestType, int requestId, string[] actionTypes)
        {
            return _dbSet.Any(r =>
                r.RequestType == requestType &&
                r.RequestId == requestId &&
                actionTypes.Contains(r.ActionType));
        }
    }
}
