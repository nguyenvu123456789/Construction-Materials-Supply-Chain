using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class MaterialCheckRepository : GenericRepository<MaterialCheck>, IMaterialCheckRepository
    {
        public MaterialCheckRepository(ScmVlxdContext context) : base(context) { }
    }
}
