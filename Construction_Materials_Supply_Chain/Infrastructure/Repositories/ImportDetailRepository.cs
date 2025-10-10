using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ImportDetailRepository : GenericRepository<ImportDetail>, IImportDetailRepository
    {
        private readonly ScmVlxdContext _context;

        public ImportDetailRepository(ScmVlxdContext context) : base(context)
        {
            _context = context;
        }

        public List<ImportDetail> GetByImportId(int importId)
        {
            return _context.ImportDetails
                .Where(d => d.ImportId == importId)
                .ToList();
        }
    }
}
