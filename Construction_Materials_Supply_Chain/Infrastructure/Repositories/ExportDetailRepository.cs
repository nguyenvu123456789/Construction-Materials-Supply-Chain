using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class ExportDetailRepository : GenericRepository<ExportDetail>, IExportDetailRepository
    {
        public ExportDetailRepository(ScmVlxdContext context) : base(context) { }

        public List<ExportDetail> GetByExportId(int exportId)
        {
            return _dbSet.Where(d => d.ExportId == exportId).ToList();
        }
    }
}
