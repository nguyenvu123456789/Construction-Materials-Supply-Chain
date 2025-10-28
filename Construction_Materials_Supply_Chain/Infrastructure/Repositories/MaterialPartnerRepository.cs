using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class MaterialPartnerRepository : GenericRepository<MaterialPartner>, IMaterialPartnerRepository
    {
        public MaterialPartnerRepository(ScmVlxdContext context) : base(context)
        {
        }
    }
}
