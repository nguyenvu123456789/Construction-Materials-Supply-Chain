using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class ExportRepository : GenericRepository<Export>, IExportRepository
    {
        public ExportRepository(ScmVlxdContext context) : base(context) { }

        public void SaveExport(Export export)
        {
            _dbSet.Add(export);
            _context.SaveChanges();
        }

        public Export GetExportById(int id)
        {
            return _dbSet.Find(id);
        }
    }
}
