using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class PartnerTypeRepository : GenericRepository<PartnerType>, IPartnerTypeRepository
    {
        public PartnerTypeRepository(ScmVlxdContext context) : base(context)
        {
        }
    }
}
