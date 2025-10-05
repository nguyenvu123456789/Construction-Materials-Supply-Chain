using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class MaterialCheckRepository : GenericRepository<MaterialCheck>, IMaterialCheckRepository
    {
        public MaterialCheckRepository(ScmVlxdContext context) : base(context) { }
    }
}
