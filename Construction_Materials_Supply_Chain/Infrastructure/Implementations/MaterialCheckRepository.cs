using Domain;
using Domain.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
{
    public class MaterialCheckRepository : GenericRepository<MaterialCheck>, IMaterialCheckRepository
    {
        public MaterialCheckRepository(ScmVlxdContext context) : base(context) { }
    }
}
