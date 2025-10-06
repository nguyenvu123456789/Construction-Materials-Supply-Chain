using Domain;
using Infrastructure.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
{
    public class ShippingLogRepository : GenericRepository<ShippingLog>, IShippingLogRepository
    {
        public ShippingLogRepository(ScmVlxdContext context) : base(context) { }
    }
}
