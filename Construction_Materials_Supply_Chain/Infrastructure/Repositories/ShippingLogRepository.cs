using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ShippingLogRepository : GenericRepository<ShippingLog>, IShippingLogRepository
    {
        public ShippingLogRepository(ScmVlxdContext context) : base(context) { }
    }
}
