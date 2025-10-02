using BusinessObjects;

namespace Repositories.Interface
{
    public interface IExportRequestRepository
    {
        List<ExportRequest> GetAll();
        ExportRequest? GetById(int id);
        ExportRequest CreateExport(ExportRequest request);
    }
}
