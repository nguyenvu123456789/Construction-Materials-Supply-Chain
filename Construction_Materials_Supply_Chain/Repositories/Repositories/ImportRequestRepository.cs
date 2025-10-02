using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ImportRequestRepository : IImportRequestRepository
    {
        private readonly ImportRequestDAO _dao;

        public ImportRequestRepository(ImportRequestDAO dao)
        {
            _dao = dao;
        }

        public List<ImportRequest> GetAll() => _dao.GetAll();

        public ImportRequest? GetById(int id) => _dao.GetById(id);

        public ImportRequest CreateImport(ImportRequest request) => _dao.CreateImport(request);
    }
}
