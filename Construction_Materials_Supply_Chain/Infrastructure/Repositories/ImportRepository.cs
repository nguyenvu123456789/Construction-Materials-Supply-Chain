using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ImportRepository : GenericRepository<Import>, IImportRepository
    {
        public ImportRepository(ScmVlxdContext context) : base(context) { }
    }
}
