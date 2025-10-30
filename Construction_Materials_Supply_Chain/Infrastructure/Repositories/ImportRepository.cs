using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ImportRepository : GenericRepository<Import>, IImportRepository
    {
        public ImportRepository(ScmVlxdContext context) : base(context) { }

        public Import? GetById(int id)
        {
            return _dbSet.FirstOrDefault(i => i.ImportId == id);
        }

        public override List<Import> GetAll()
        {
            return _dbSet.Include(i => i.ImportDetails).ToList();
        }

        public Import? GetByIdWithDetails(int id)
        {
            return _dbSet.Include(i => i.ImportDetails)
                         .FirstOrDefault(i => i.ImportId == id);
        }
        public List<Import> GetAllWithWarehouse()
        {
            return _context.Imports
                .Include(i => i.Warehouse)
                    .ThenInclude(w => w.Manager)
                .Include(i => i.ImportDetails)
                .ToList();
        }

    }
}