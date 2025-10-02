using BusinessObjects;

namespace Repositories.Interface
{
    public interface IImportRequestRepository
    {
        List<ImportRequest> GetAll();
        ImportRequest? GetById(int id);
        ImportRequest CreateImport(ImportRequest request);
    }
}
