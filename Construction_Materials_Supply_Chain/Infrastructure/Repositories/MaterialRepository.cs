using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(ScmVlxdContext context) : base(context) { }
    }
}
