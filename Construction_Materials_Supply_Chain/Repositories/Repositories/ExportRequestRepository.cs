using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ExportRequestRepository : IExportRequestRepository
    {
        private readonly ExportRequestDAO _dao;

        public ExportRequestRepository(ExportRequestDAO dao)
        {
            _dao = dao;
        }

        public List<ExportRequest> GetAll() => _dao.GetAll();

        public ExportRequest? GetById(int id) => _dao.GetById(id);

        public ExportRequest CreateExport(ExportRequest request) => _dao.CreateExport(request);
    }
}
