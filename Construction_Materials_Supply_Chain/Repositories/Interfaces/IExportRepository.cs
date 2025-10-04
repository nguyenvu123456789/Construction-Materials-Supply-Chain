using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interface
{
    public interface IExportRepository : IGenericRepository<Export>
    {
        void SaveExport(Export export);
        Export GetExportById(int id);
    }
}
