using BusinessObjects;
using System.Collections.Generic;

namespace Repositories.Interface
{
    public interface IExportRepository
    {
        void SaveExport(Export export);
        Export GetExportById(int id);
        List<Export> GetExports();
    }
}
