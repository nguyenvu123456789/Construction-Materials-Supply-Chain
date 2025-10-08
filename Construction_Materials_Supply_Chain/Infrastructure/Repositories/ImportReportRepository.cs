using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ImportReportRepository : GenericRepository<ImportReport>, IImportReportRepository
    {
        public ImportReportRepository(ScmVlxdContext context) : base(context) { }
    }
}
