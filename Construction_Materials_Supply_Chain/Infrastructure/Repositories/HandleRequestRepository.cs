using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class HandleRequestRepository : GenericRepository<HandleRequest>, IHandleRequestRepository
    {
        public HandleRequestRepository(ScmVlxdContext context) : base(context) { }

        public List<HandleRequest> GetByRequest(string requestType, int requestId)
        {
            return _dbSet.Where(r => r.RequestType == requestType && r.RequestId == requestId).ToList();
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
