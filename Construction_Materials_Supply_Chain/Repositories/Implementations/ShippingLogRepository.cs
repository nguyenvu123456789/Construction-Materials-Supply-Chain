using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class ShippingLogRepository : GenericRepository<ShippingLog>, IShippingLogRepository
    {
        public ShippingLogRepository(ScmVlxdContext context) : base(context) { }
    }
}
