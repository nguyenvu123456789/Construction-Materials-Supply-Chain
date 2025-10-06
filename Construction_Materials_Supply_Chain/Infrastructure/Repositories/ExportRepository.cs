using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
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
