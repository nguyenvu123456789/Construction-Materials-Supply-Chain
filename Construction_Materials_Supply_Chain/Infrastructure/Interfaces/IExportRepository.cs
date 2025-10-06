using Domain;
using Infrastructure.Base;

namespace Infrastructure.Interface
{
    public interface IExportRepository : IGenericRepository<Export>
    {
        void SaveExport(Export export);
        Export GetExportById(int id);
    }
}
