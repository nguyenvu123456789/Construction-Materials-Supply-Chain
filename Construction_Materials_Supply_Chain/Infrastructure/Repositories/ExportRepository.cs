using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ExportRepository : GenericRepository<Export>, IExportRepository
    {
        public ExportRepository(ScmVlxdContext context) : base(context) { }

        public Export GetExportById(int id)
        {
            return _dbSet.Include(e => e.ExportDetails).FirstOrDefault(e => e.ExportId == id);
        }

        public override List<Export> GetAll()
        {
            return _dbSet.Include(e => e.ExportDetails).ToList();
        }
    }
}