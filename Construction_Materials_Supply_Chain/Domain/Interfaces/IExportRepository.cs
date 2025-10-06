using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IExportRepository : IGenericRepository<Export>
    {
        void SaveExport(Export export);
        Export GetExportById(int id);
    }
}
