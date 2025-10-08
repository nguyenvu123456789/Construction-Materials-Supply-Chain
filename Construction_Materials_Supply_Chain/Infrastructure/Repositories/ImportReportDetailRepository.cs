using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ImportReportDetailRepository : GenericRepository<ImportReportDetail>, IImportReportDetailRepository
    {
        public ImportReportDetailRepository(ScmVlxdContext context) : base(context) { }
    }
}
