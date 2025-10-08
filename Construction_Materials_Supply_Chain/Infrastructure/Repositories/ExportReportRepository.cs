using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class ExportReportRepository : GenericRepository<ExportReport>, IExportReportRepository
    {
        public ExportReportRepository(ScmVlxdContext context) : base(context) { }

        // Các method custom nếu cần
    }
}
