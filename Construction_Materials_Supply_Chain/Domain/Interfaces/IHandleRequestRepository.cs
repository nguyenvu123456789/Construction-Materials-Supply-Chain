using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IHandleRequestRepository : IGenericRepository<HandleRequest>
    {
        List<HandleRequest> GetByRequest(string requestType, int requestId);
        bool Exists(string requestType, int requestId, string[] actionTypes);

    }
}
