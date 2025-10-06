using Domain;
using Domain.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
{
    public class PartnerTypeRepository : GenericRepository<PartnerType>, IPartnerTypeRepository
    {
        public PartnerTypeRepository(ScmVlxdContext context) : base(context)
        {
        }
    }
}
